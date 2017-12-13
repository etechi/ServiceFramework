import React = require('react');
import { render } from 'react-dom'
import { Router, browserHistory, Route, IndexRoute,PlainRoute } from 'react-router'
//import formItemAdjuster = require("./formItemAdjuster");

import * as WA from "SF/webadmin";
import api = require("./webapi-all");
import assign = require("SF/utils/assign");
import Meta = require("SF/utils/Metadata");
import ApiMeta = require("SF/utils/ApiMeta");
import ApiFormManager = require("SF/components/webapi/ApiFormManager");
import ApiTableManager = require("SF/components/webapi/ApiTableManager");
import { setup as setupEntityLinkBuilder } from "SF/utils/EntityLinkBuilder";
import apicall = require("SF/utils/apicall");
api.setApiInvoker(apicall.call);

declare var require: any;
var env = (window as any)["ENV"];

function init(
    lib: ApiMeta.Library,
    menuItems: api.SF$Sys$MenuServices$MenuItem[],
    permissions: any//api.ServiceProtocol$Auth$Permission[]
) {
    //var modules=require("./modules").default;
    var config = require("./config");
    var AppFrame=require("./components/AppFrame").default;
    var Dashboard = require("./components/Dashboard").default;
    var SigninPage = require("./components/SigninPage").default;

    ApiMeta.setDefaultLibrary(lib);

    var apiForms = new ApiFormManager.ApiFormManager(lib, null);//formItemAdjuster.itemAdjuster);
    //modules.filter(m => m.api && m.api.forms ? true : false).forEach(m =>
    //    m.api.forms.forEach(f => apiForms.createForm(f))
    //);
    ApiFormManager.setDefaultFormManager(apiForms);

    
    config.build(lib,env.root, permissions, menuItems, window.location.href.indexOf("all=true")!=-1);

 
    //const module_routes = modules.map(m => m.route);
    setupEntityLinkBuilder([config.ManagerBuildResult.entityLinkBuilders], env.root);

    var tm = new ApiTableManager.ApiTableManager(apiForms);
    //modules.filter(m => m.api && m.api.queries ? true : false).forEach(m =>
    //    m.api.queries.forEach(f => tm.createTable(f))
    //);
    ApiTableManager.setDefaultTableManager(tm);

    //var reducer = (state, action) => {
    //    if (state === void 0) { state = []; }
    //    switch (action.type) {
    //        default:
    //            return state;
    //    }
    //};
    //var form_normalizers = assign(
    //    apiForms.normalizer(),
    //    settingForms.normalizer()
    //    );
    //var reducers = combineReducers({
    //    reducer: reducer,
    //    //form:  formReducer.normalize(form_normalizers),
    //});
    //var store = createStore(reducers);

    const chd_routes: PlainRoute[] = [
        {
            path: "dashboard",
            component: Dashboard
        },
        {
            path: "signin",
            component: SigninPage
        }

    ];


    const routes = {
        path: env.root,
        component: AppFrame,
        indexRoute: {
            onEnter: (nextState, transition) => {
                transition(env.root+'dashboard');
            }
        },
        childRoutes: chd_routes
            //.concat(config.ManagerBuildResult.entityRoutes)
            //.concat(config.ManagerBuildResult.formRoutes)
            .concat(
                config.ManagerBuildResult.routes
            )
            .concat([
            //{
            //    path: "entity/:type/*",
            //    onEnter: (nextState, transition) => {
            //        var { location, params } = nextState;
            //        var builder = entityMap[params.type];
            //        if (!builder) return;
            //        var to = "/admin/" + builder.apply(null, params.splat.split('/'));
            //        transition(to);
            //    }
            //}
        ])
    }

    render((
        <Router history={browserHistory} routes={routes}>
        </Router>
    ), document.getElementById('app'))

}
Promise.all([
    api.ServiceMetadata.Json(),
    api.Menu.GetMenu(env.menu),

    //api.User.GetPermissions() 
]).then(re => {
    var lib = new ApiMeta.Library(re[0] as any);
    var items = re[1];
    var permissions = re[2];
    init(lib, items,permissions);
});


function removeQuery(s: string) {
    if (!s) return s;
    var i = s.indexOf('?');
    return i == -1 ? s : s.substring(0, i);
}

browserHistory.listen(l => {
    //var loc = window.location.href;
    //var re = loc.match(/^https?:\/\/[^\/]+\/admin\/[^\/]+\/([^\/]+)$/);
    //var res = removeQuery(re && re[1]);
    //if (re && res) {
    //    api.AuditService.AddRecord({
    //        Resource: decodeURIComponent(res),
    //        Operation: "查询"
    //    });
    //    return;
    //}

    //var re = loc.match(/^https?:\/\/[^\/]+\/admin\/[^\/]+\/([^\/]+)\/([^\/]+)\/([^\/]+)$/);
    //var id = removeQuery(re && re[3]);
    //if (re && id) {
    //    api.AuditService.AddRecord({
    //        DestId: decodeURIComponent(re[1]) + "-" + decodeURIComponent(id),
    //        Resource: decodeURIComponent(re[1]),
    //        Operation: "浏览"
    //    });
    //    return;
    //}
});
