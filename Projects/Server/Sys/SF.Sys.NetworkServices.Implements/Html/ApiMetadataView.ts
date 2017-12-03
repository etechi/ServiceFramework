///<reference path="../../Scripts/typings/angularjs/angular.d.ts"/>

namespace metadataView {
    var app = angular.module("metadata",[]);

    interface AttributeValue {
        Name: string;
        Value: string;
    }
    interface Attribute {
        Type: string;
        Values: AttributeValue[];
    }
    interface Entity {
        Attributes: Attribute[];
        Name: string;
        Title: string;
        Description: string;
        Group: string;
        Prompt: string;
        ShortName: string;
    }
    interface Type extends Entity {
        BaseTypes: string[];
        ElementType: string;
        IsEnumType: boolean;
        IsArrayType: boolean;
        Properties: Property[];
    }
    interface Property extends Entity {
        Type: string;
        Optional: boolean;
    }
    interface Controller extends Entity {
        ActionGroups: ActionGroup[];
        Actions: Action[];
        UserRequired: boolean;
        RolesRequired: string[];
        PermissionsRequired: string[];
    }

    interface Action extends Entity {
        Parameters: Parameter[];
        Type: string;
        HttpMethods: string[];
        UserRequired: boolean;
        RolesRequired: string[];
        PermissionsRequired: string[];
    }

    interface Parameter extends Entity {
        Optional: boolean;
        FromBody: boolean;
        Type: string;
    }
    interface Library {
        Types: Type[];
        Controllers: Controller[];
    }
    interface Group<T> {
        Name: string;
        Items:T[]
    }
    interface ControllerGroup extends Group<Controller>{
    }
    interface ActionGroup extends Group<Action> {
    }
    interface topScope extends angular.IScope {
        cur_controller: Controller;
        cur_action: Action;
        cur_action_types: Type[];
        set_cur_controller(c: Controller): void;
        set_cur_action(a: Action): void;
        get_controller_groups(): ControllerGroup[];
        get_controllers(cg: ControllerGroup): Controller[];
        get_action_groups(c: Controller): ActionGroup[];
        get_actions(ag: ActionGroup): Action[];
    }
    app.controller("top", ["$scope", "$http", ($scope: topScope, $http: ng.IHttpService) => {

        var controller_groups: ControllerGroup[] = [];
        var types: { [index: string]: Type } = null;

        $scope.set_cur_action = (a) => {
            $scope.cur_action = a;
            $scope.cur_action_types = [];
            var type_hash: any = {};

            const add_type = (id: string) => {
                var t = types[id];
                if (!t) return;
                if (t.IsArrayType) {
                    if (type_hash[t.Name]) return;
                    type_hash[t.Name] = true;
                    $scope.cur_action_types.push(t);
                    add_type(t.ElementType);
                    return;
                }
                if (!t.Properties && !t.BaseTypes ) return;
                if (type_hash[t.Name]) return;
                type_hash[t.Name] = true;
                $scope.cur_action_types.push(t);
                if (t.BaseTypes) t.BaseTypes.forEach(add_type);
                t.Properties.forEach(p => add_type(p.Type));
            }
            add_type(a.Type);
            $scope.cur_action.Parameters.forEach(p =>add_type(p.Type));


        }
        $scope.set_cur_controller = (a) =>
            $scope.cur_controller = a;

        $scope.get_controller_groups = () =>
            controller_groups;

        $scope.get_controllers = (cg: ControllerGroup) =>
            cg && cg.Items;

        $scope.get_action_groups = (c: Controller) =>
            c && c.ActionGroups;

        $scope.get_actions = (ag: ActionGroup) =>
            ag && ag.Items;


        function group<T extends Entity, G extends Group<T>>(items: T[]): G[] {
            var cg_set = items.reduce((set, i) => {
                var grp = i.Group || "[默认分组]";
                var ns = set[grp];
                if (!ns) set[grp] = ns = <G>{ Name: grp, Items: <T[]>[] };
                ns.Items.push(i);
                return set;
            }, <{ [index: string]: G }>{});

            var cgs: G[] = [];
            for (var k in cg_set)
                cgs.push(cg_set[k]);
            cgs.sort((x, y) => x.Name.localeCompare(y.Name));
            return cgs;
        }

        $http.get("json").success((re: Library) => {
            types = {};
            re.Types.forEach(t => types[t.Name] = t);

            controller_groups = group(re.Controllers);
            controller_groups.forEach(cg => {
                cg.Items.forEach(c => {
                    c.ActionGroups = group(c.Actions);
                });
            });
        });
    }]);
} 