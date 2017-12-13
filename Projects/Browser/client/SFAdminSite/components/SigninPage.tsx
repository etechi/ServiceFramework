import React = require('react')

import api = require("../webapi-all");

export interface SigninPageProps {
} 

interface state {
    account?: string;
    password?: string;
    msg?: string;
    executing?: boolean;
}
export default class SigninPage extends React.Component<SigninPageProps, state> {
    constructor(props: SigninPageProps) {
        super(props);
       this.state = {};
    }
    handleSignin() {
        this.setState({ executing: true });
        var env = (window as any)["ENV"];
        var sid=env.identityServiceId;

        api.User.Signin({
            Ident: this.state.account,
            Password: this.state.password,
            ReturnToken: false
        }, {
              serviceId:sid  
            }).then(re => {
            this.setState({ executing: false });
        }, err => {
            this.setState({
                executing: false,
                msg:err._error
            });
        });
    }
    render() {
        var s = this.state;
        return <div>
            <br/>
            <p>请您先登录管理系统：</p>
            <br />
            <form className="dynamic-form ">

                <div className="form-group clearfix field-Entity">

                    <div className="control-content ">
                        <div className="form-item-set">
                            <div className="vbox">
                                <div className="form-group clearfix field-Name">
                                    <label className="control-label">账号</label>
                                    <div className="control-content field-size-sm">
                                        <input type="text" className="form-control" placeholder="请输入账号" maxLength={50} value={s.account} onChange={(e) => this.setState({ account: e.target.value })} />
                                    </div>
                                </div>
                                <div className="form-group clearfix field-Name">
                                    <label className="control-label">密码</label>
                                    <div className="control-content field-size-sm">
                                        <input type="password" className="form-control" maxLength={50} value={s.password} onChange={(e) => this.setState({ password: e.target.value })} />
                                    </div>
                                </div>
                                {s.msg ? <div className="form-group clearfix field-Name">
                                    <label className="control-label">&nbsp;</label>
                                    <div className="control-content">
                                        {s.msg}
                                    </div>
                                </div> : null}
                                <div className="form-group clearfix field-Name">
                                    <label className="control-label">&nbsp;</label>
                                    <div className="control-content field-size-sm">
                                        <button onClick={() => this.handleSignin()} type="button" className="btn btn-primary">
                                            <span className={['fa', 'fa-search'].concat(s.executing?["fa-spin"]:[]).join(' ')}></span>
                                            登录
                                        </button>
                                    </div>
                                </div>
                                
                            </div>
                        </div>
                    </div>
                </div>

            </form>
        </div>
    }
}