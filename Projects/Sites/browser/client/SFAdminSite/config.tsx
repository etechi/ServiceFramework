//import {PlainRoute} from 'react-router';
//import * as menu from 'service-protocol-webadmin/lib/menu-types';
//import * as EntityLinkBuilder from "service-protocol-react/lib/utils/EntityLinkBuilder";
//import * as ApiMeta from "service-protocol-react/lib/utils/ApiMeta";
//import * as Views from 'service-protocol-webadmin/lib/components/Views';
import ManagerBuilder = require("SF/webadmin/ManagerBuilder"); 
import * as ApiMeta from "SF/utils/ApiMeta";
import api = require("./webapi-all");

//var help_accounting = require("./help/accounting/index.md");
//var help_delivery = require("./help/delivery/index.md");
//var help_doc = require("./help/doc/index.md");
//var help_product = require("./help/product/index.md");
//var help_promotion = require("./help/promotion/index.md");
//var help_round = require("./help/round/index.md");
//var help_sysservice = require("./help/sysservice/index.md");
//var help_trade = require("./help/trade/index.md");
//var help_uimanager = require("./help/uimanager/index.md");
//var help_user = require("./help/user/index.md");
 
var cfg: ManagerBuilder.IManagerConfig = {
    urlRoot:"/",
    groups: [
        //{
        //    title: "",
        //    modules: [
        //        {
        //            title: "订单管理",
        //            icon: "fa fa-shopping-cart",
        //            items: [
        //                { type: "entity", source: "交易", icon: "fa fa-list-alt", title: "订单查询" },
        //                { type: "entity", source: "交易项目", icon: "fa fa-list-alt", title: "订单明细查询" },
        //                { type: "help", source: help_trade}
        //           ]
        //        },
        //        {
        //            title: "夺宝管理",
        //            icon: "fa fa-trophy",
        //            items: [
        //                { type: "entity", source: "夺宝轮次", icon: "fa fa-flag-o", title: "夺宝轮次查询" },
        //                { type: "entity", source: "夺宝中奖纪录", icon: "fa fa-flag-o", title: "中奖纪录查询" },
        //                { type: "entity", source: "晒单", icon: "fa fa-flag-o", title: "用户晒单查询", entityMode: ManagerBuilder.EntityItemMode.NoCreate},
        //                { type: "entity", source: "夺宝调整设置", icon: "fa fa-flag-o", title: "夺宝调整设置" },
        //                { type: "entity", source: "夺宝调整记录", icon: "fa fa-flag-o", title: "夺宝调整记录" },
        //                { type: "entity", source: "彩票断网请求", icon: "fa fa-flag-o", title: "彩票断网记录" },
        //                { type: "help", source: help_round }

        //            ]
        //        },
        //        {
        //            title: "促销管理",
        //            icon: "fa fa-bullhorn",
        //            items: [
        //                //{ type: "entity", source: "积分", icon: "fa fa-star-o", title:"积分管理" },
        //                //{ type: "entity", source: "积分记录", icon:"fa fa-star-half-o" },
        //                //{ type: "entity", source: "积分等级",icon:"fa fa-signal" },
        //                { type: "entity", source: "活动", icon:"fa fa-rocket"},
        //                { type: "entity", source: "活动记录", icon:"fa fa-list-ol"},
        //                { type: "entity", source: "活动获奖记录", icon: "fa fa-list-ol"},
        //                { type: "entity", source: "专属活动", icon: "fa fa-user-secret" },
        //                { type: "entity", source: "专属活动参与记录", icon: "fa fa-user-secret" },
        //                { type: "entity", source: "优惠券", icon: "fa fa-barcode" },
        //                { type: "entity", source: "优惠券获取记录", icon: "fa fa-barcode" },
        //                { type: "entity", source: "优惠券使用记录", icon: "fa fa-barcode" },
        //                { type: "entity", source: "优惠券模板", icon:"fa fa-clone" },
        //                { type: "entity", source: "实物奖品", icon:"fa fa-gift" },
        //                { type: "entity", source: "实物获奖记录", icon: "fa fa-list" },
                        
        //                { type: "help", source: help_promotion }

        //            ]
        //        },
        //        {
        //            title: "产品管理",
        //            icon: "fa fa-diamond",
        //            items: [
        //                { type: "entity", source: "产品", icon:"fa fa-list"},
        //                { type: "entity", source: "产品类型", icon:"fa fa-cubes" },
        //                { type: "entity", source: "产品目录", icon:"fa fa-server" },
        //                { type: "help", source: help_product}
        //            ]
        //        },
        //        {
        //            title: "发货管理", 
        //            icon: "fa fa-truck",
        //            items: [
        //                { type: "entity", source: "发货", icon:"fa fa-send-o"},
        //                { type: "entity", source: "发货地址", icon:"fa fa-building-o"},
        //                { type: "entity", source: "发货渠道", icon: "fa fa-plane" },
        //                { type: "entity", source: "虚拟项目自动发货规格", icon: "fa fa-plane", title: "自动发货规格"},
        //                { type: "entity", source: "虚拟项目自动发货导入批次", icon: "fa fa-plane", title: "导入自动发货数据" },
        //                { type: "entity", source: "虚拟项目自动发货导入记录", icon: "fa fa-plane", title: "自动发货导入记录查询" },
        //                { type: "entity", source: "虚拟项目自动发货记录", icon: "fa fa-plane", title: "自动发货记录查询"},
        //                { type: "open", title:"快递查询", source: "http://www.kuaidi100.com/frame/910.html", icon: "fa fa-search" },
        //                { type: "help", source: help_delivery }
        //            ]
        //        },
        //        {
        //            title: "用户管理",
        //            icon: "fa fa-user",
        //            items: [
        //                { type: "entity", source: "用户", icon: "fa fa-users" },
        //                { type: "entity", source: "用户渠道", icon: "fa fa-road" },
        //                { type: "entity", source: "模拟用户", icon: "fa fa-users" },
        //                { type: "help", source: help_user }
        //            ]
        //        },
        //        {
        //            title: "财务管理",
        //            icon: "fa fa-money",
        //            items: [
        //                { type: "entity", source: "账户" },
        //                { type: "entity", source: "账户充值记录" },
        //                { type: "entity", source: "账户转账记录" },
        //                { type: "entity", source: "账户退款记录" },
        //                { type: "entity", source: "账户科目" },
        //                { type: "help", source: help_accounting }
        //            ]
        //        }


        //    ]

        //},
        //{
        //    title: "客服管理",
        //    modules: [
        //        {
        //            title: "用户反馈",
        //            items: [
        //                { type: "entity", source: "用户反馈", entityMode: ManagerBuilder.EntityItemMode.NoCreate }
        //            ] 
        //        },
        //        {
        //            title: "通知管理",
        //            items: [
        //                { type: "entity", source: "全体通知" },
        //                { type: "entity", source: "普通通知" }
        //            ]
        //        }

        //    ]

        //},
        //{
        //    title: "网站管理",
        //    modules: [
        //        {
        //            title: "常用设置",
        //            items: [
        //                { type: "form", source: "UIManagerFriendlyManager/PCHeadMenu", icon: "fa fa-align-left", },
        //                { type: "form", source: "UIManagerFriendlyManager/PCHeadProductCategory", icon: "fa fa-align-left",},
        //                { type: "form", source: "UIManagerFriendlyManager/PCHomeSlider", icon: "fa fa-align-left",},
        //                { type: "form", source: "UIManagerFriendlyManager/PCTailMenu", icon: "fa fa-align-left",},
        //                { type: "list", source: "UIManagerAdManager/PCAdArea", icon: "fa fa-align-left", },

        //                { type: "form", source: "UIManagerFriendlyManager/MobileProductCategory", icon: "fa fa-align-left", },
        //                { type: "form", source: "UIManagerFriendlyManager/MobileHomeMenu", icon: "fa fa-align-left",},
        //                { type: "form", source: "UIManagerFriendlyManager/MobileHomeSlider", icon: "fa fa-align-left", }
        //            ]
        //        },
        //        {
        //            title: "高级页面管理",
        //            icon: "fa fa-magic",
        //            items: [
        //                { type: "entity", source: "界面内容", icon: "fa fa-align-left", hidden:true},
        //                { type: "entity", source: "界面站点", icon: "fa fa-globe", hidden: true },
        //                { type: "entity", source: "界面站点模板", icon: "fa fa-sitemap", hidden: true }
        //                //{ type: "help", source: help_uimanager, hidden: true }
        //            ]
        //        },
        //        {
        //            title: "文档管理",
        //            icon: "fa fa-file-image-o",
        //            items: [
        //                { type: "entity", source: "文档", icon: "fa fa-list" },
        //                { type: "entity", source: "文档目录", icon: "fa fa-folder-open-o" },
        //                { type: "entity", source: "文档分区", icon: "fa fa-th-large" },
        //                { type: "help", source: help_doc}
        //            ]
        //        }

        //    ]

        //},
        {
            title: "系统管理",
            modules: [
                //{
                //    title: "系统安全",
                //    items: [
                //        { type: "entity", source: "管理员" },
                //        { type: "entity", source: "用户角色" },
                //        { type: "form", source: "AuthAdminManager/AdminInfo", hidden:true }
                //    ]
                //},
                //{
                //    title: "系统日志",
                //    icon: "fa fa-magic",
                //    items: [
                //        { type: "entity", source: "文本消息记录" },
                //        { type: "entity", source: "支付收款记录" },
                //        { type: "entity", source: "支付退款记录" },
                //        { type: "entity", source: "安全审核记录" }
                //    ]
                //},
                //{
                //    title: "系统设置",
                //    icon: "fa fa-server",
                //    items: [
                //        { type: "setting", source: null, hidden: true}
                //    ]
                //},
                {
                    title: "系统服务管理",
                    items: [
                        { type: "entity", source: "系统服务定义" },
                        { type: "entity", source: "系统服务实现" },
                        { type: "entity", source: "系统服务实例" },
                        //{ type: "entity", source: "系统服务", hidden: true },
                        //{ type: "entity", source: "系统服务类型", hidden: true},
                        //{ type: "entity", source: "系统服务提供者" ,hidden: true },
                        //{ type: "entity", source: "系统服务协议", hidden: true}
                    ]
                }
            ]
        }
    ]
};

function itemNormalize(items: api.SF$Management$MenuServices$Models$MenuItem[]): api.SF$Management$MenuServices$Models$MenuItem[] {
    var re: api.SF$Management$MenuServices$Models$MenuItem[] = [];
    var l2: api.SF$Management$MenuServices$Models$MenuItem[] = [];
    items.forEach(i => {
        if (i.Children[0].Children)
            re.push(i);
        else
            l2.push(i);
    });
    re.unshift({Id: 0, ObjectState: "Enabled", Name: "", Title: "", Children: l2, Action: "None" } as any);
    return re;
}
function newItem(item: api.SF$Management$MenuServices$Models$MenuItem): ManagerBuilder.IItemConfig {
    switch (item.Action) {
        case "Link":
            return {
                type: "open", title: item.Title || item.Name, source: item.ActionArgument
            };
        case "EntityManager":
            return {
                type: "entity", title: item.Title || item.Name, source: item.ActionArgument
            };
        default:
            return null;
    }
}
function newModule(item: api.SF$Management$MenuServices$Models$MenuItem): ManagerBuilder.IModuleConfig {
    var items = item.Children.map(c => newItem(c));
    return {
        title: item.Title || item.Name,
        items: items
    };
}
function newGroup(item: api.SF$Management$MenuServices$Models$MenuItem): ManagerBuilder.IGroupConfig {
    var modules = item.Children.map(c => newModule(c));
    return {
        title: item.Title || item.Name,
        modules: modules
    }; 

}
export var ManagerBuildResult: ManagerBuilder.IManagerBuildResult = null;
export function build(lib: ApiMeta.Library, permissions: ManagerBuilder.IPermission[], items: api.SF$Management$MenuServices$Models$MenuItem[], all?: boolean) {
    if (items && items.length) {
        items = itemNormalize(items);
        cfg.groups = items.map(i => newGroup(i));
    }
    ManagerBuildResult = ManagerBuilder.buildManager(lib, cfg, permissions, all);
    
}
