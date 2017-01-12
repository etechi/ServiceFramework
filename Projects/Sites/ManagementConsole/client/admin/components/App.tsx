import React = require('react')
import Dashboard from './Dashboard'
import * as WA from "SF/webadmin";
import {Image} from "SF/components/utils/Image";
//import modules from "../modules";
import api = require("../webapi-all");
//import * as auth from "../auth";
import * as config from "../config";

var auth: any;

export interface AppProps {
    children?: JSX.Element[]
}
interface state {
}
export default class App extends React.Component<AppProps, state>
{
    static contextTypes = {
        router: React.PropTypes.object.isRequired
    };
    constructor(props: AppProps,ctx:any) {
        super(props, ctx);
    }
    handleSignout() {
        //api.User.Signout().then(() => {
        //    window.location.href = "/admin/signin";
        //});
    }
    render() {
        var u :any= {};//auth.user();
        return <WA.Application>
            <WA.Header.Container>
                <WA.Header.Logo>好云购管理中心</WA.Header.Logo>
                <WA.Header.Text to={"/admin/" + encodeURIComponent("系统安全") + "/AdminInfo"}>
                    <Image className="img-circle" format="c30" res={u.icon} />
                    <span className="username username-hide-on-mobile">{u.nick}</span>
                </WA.Header.Text>
                <WA.Header.Button onClick={() => this.handleSignout() }><i className="icon-logout"></i></WA.Header.Button>
            </WA.Header.Container>
            <WA.SideBar.Container pathPrefix="/admin" menuCategories={config.ManagerBuildResult.menus/* modules.map(m => m.menu) */} >
                {/*<WA.SideBar.SearchBox></WA.SideBar.SearchBox>*/}
                <WA.SideBar.MenuItem icon='icon-home' name='首页' to='/admin/dashboard' />
            </WA.SideBar.Container>
            {/*<WA.Footer>footer</WA.Footer>*/}
            {this.props.children}
        </WA.Application>
    }
} //<WA.SideBarMenuCategory  name="财务管理" icon="icon-user">
                //    <WA.SideBarMenuItem name="支付清单" icon="icon-calendar" to="/account/order"/>
                //    <WA.SideBarMenuItem name="调账" icon="icon-refresh" to="/account/adjuest"/>
                //    <WA.SideBarMenuItem name="账单查询" icon="icon-calendar" to="/account/query"/>
                //</WA.SideBarMenuCategory>

                //<WA.SideBarMenuCategory  name="会员管理" icon="icon-user">
                //    <WA.SideBarMenuItem name="会员列表" icon="icon-puzzle" to="/user/list"/>
                //    <WA.SideBarMenuItem name="修改资料" icon="icon-refresh" to="/user/modify"/>
                //    <WA.SideBarMenuItem name="重置密码" icon="icon-calendar" to="/user/password-reset"/>
                //</WA.SideBarMenuCategory>

                //<WA.SideBarMenuGroup name='系统管理'/>

                //<WA.SideBarMenuCategory  name="网站管理" icon="icon-user">
                //    <WA.SideBarMenuItem name="新闻管理" icon="icon-puzzle" to="/sites/news/list"/>
                //    <WA.SideBarMenuItem name="帮助管理" icon="icon-refresh" to="/sites/help/list"/>
                //    <WA.SideBarMenuItem name="留言管理" icon="icon-refresh" to="/sites/comment/list"/>
                //    <WA.SideBarMenuItem name="页面区域管理" icon="icon-calendar" to="/sites/ui/list"/>
                //</WA.SideBarMenuCategory>
                //<WA.SideBarMenuCategory name="安全中心" icon="icon-user">
                //    <WA.SideBarMenuItem name="管理员管理" icon="icon-puzzle" to="/security/admin/list"/>
                //    <WA.SideBarMenuItem name="角色管理" icon="icon-refresh" to="/security/role/list"/>
                //    <WA.SideBarMenuItem name="权限管理" icon="icon-refresh" to="/security/permission/list"/>
                //    <WA.SideBarMenuItem name="操作日志" icon="icon-calendar" to="/security/operation/list"/>
                //</WA.SideBarMenuCategory>
                //<WA.SideBarMenuCategory  name="系统设置" icon="icon-user">
                //    <WA.SideBarMenuItem name="管理员管理" icon="icon-puzzle" to="/sys/a"/>
                //    <WA.SideBarMenuItem name="角色管理" icon="icon-refresh" to="/sys/b"/>
                //    <WA.SideBarMenuItem name="权限管理" icon="icon-refresh" to="/sys/c"/>
                //    <WA.SideBarMenuItem name="操作日志" icon="icon-calendar" to="/sys/d"/>
                //</WA.SideBarMenuCategory>