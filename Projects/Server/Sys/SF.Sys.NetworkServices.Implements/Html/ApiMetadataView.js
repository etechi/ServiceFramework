///<reference path="../../Scripts/typings/angularjs/angular.d.ts"/>
var metadataView;
(function (metadataView) {
    var app = angular.module("metadata", []);
    app.controller("top", ["$scope", "$http", function ($scope, $http) {
            var controller_groups = [];
            var types = null;
            $scope.set_cur_action = function (a) {
                $scope.cur_action = a;
                $scope.cur_action_types = [];
                var type_hash = {};
                var add_type = function (id) {
                    var t = types[id];
                    if (!t)
                        return;
                    if (t.IsArrayType) {
                        if (type_hash[t.Name])
                            return;
                        type_hash[t.Name] = true;
                        $scope.cur_action_types.push(t);
                        add_type(t.ElementType);
                        return;
                    }
                    if (!t.Properties && !t.BaseTypes)
                        return;
                    if (type_hash[t.Name])
                        return;
                    type_hash[t.Name] = true;
                    $scope.cur_action_types.push(t);
                    if (t.BaseTypes)
                        t.BaseTypes.forEach(add_type);
                    t.Properties.forEach(function (p) { return add_type(p.Type); });
                };
                add_type(a.Type);
                $scope.cur_action.Parameters.forEach(function (p) { return add_type(p.Type); });
            };
            $scope.set_cur_controller = function (a) {
                return $scope.cur_controller = a;
            };
            $scope.get_controller_groups = function () {
                return controller_groups;
            };
            $scope.get_controllers = function (cg) {
                return cg && cg.Items;
            };
            $scope.get_action_groups = function (c) {
                return c && c.ActionGroups;
            };
            $scope.get_actions = function (ag) {
                return ag && ag.Items;
            };
            function group(items) {
                var cg_set = items.reduce(function (set, i) {
                    var grp = i.Group || "[默认分组]";
                    var ns = set[grp];
                    if (!ns)
                        set[grp] = ns = { Name: grp, Items: [] };
                    ns.Items.push(i);
                    return set;
                }, {});
                var cgs = [];
                for (var k in cg_set)
                    cgs.push(cg_set[k]);
                cgs.sort(function (x, y) { return x.Name.localeCompare(y.Name); });
                return cgs;
            }
            $http.get("json").success(function (re) {
                types = {};
                re.Types.forEach(function (t) { return types[t.Name] = t; });
                controller_groups = group(re.Services);
                controller_groups.forEach(function (cg) {
                    cg.Items.forEach(function (c) {
                        c.ActionGroups = group(c.Methods);
                    });
                });
            });
        }]);
})(metadataView || (metadataView = {}));
//# sourceMappingURL=ApiMetadataView.js.map