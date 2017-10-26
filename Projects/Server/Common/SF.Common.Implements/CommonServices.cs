#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0


using SF.Core.Hosting;
using SF.Core.ServiceManagement.Management;
using SF.Management.MenuServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class CommonServices
	{
		public static IServiceCollection AddCommonServices(this IServiceCollection Services, EnvironmentType EnvType)
		{
			Services.AddTextMessageServices();

			Services.AddMenuService();

			Services.AddAuthUserServices();
			Services.AddMemberServices();

			Services.AddAdminServices();
			//Services.AddBizAdminServices();
			Services.AddFriendlyFrontEndServices();

			Services.AddDocumentServices();
			
			Services.InitServices("系统服务", InitServices);
			return Services;
		}
		static async Task InitServices(IServiceProvider sp,IServiceInstanceManager sim,long? ParentId)
		{
			var SysAdminService = await sim.NewAdminService().Ensure(sp, ParentId);
			//var BizAdminService = await sim.NewBizAdminService().Ensure(sp, ParentId);
			var MenuService = await sim.NewMenuService().Ensure(sp, ParentId);
			var MemberService = await sim.NewMemberService().Ensure(sp,ParentId);

			//var bizMenuItems = new[]
			//{
			//	new MenuItem
			//	{
			//		Name="会员管理",
			//		Children = new[]
			//		{
			//			new MenuItem
			//			{
			//				Name="会员",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			},
			//			new MenuItem
			//			{
			//				Name="渠道管理",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			},
			//			new MenuItem
			//			{
			//				Name="模拟会员",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			}
			//		}
			//	},
			//	new MenuItem
			//	{
			//		Name="产品管理",
			//		Children=new[]{
			//			new MenuItem
			//			{
			//				Name="产品",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			},
			//			new MenuItem
			//			{
			//				Name="产品类型",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			},
			//			new MenuItem
			//			{
			//				Name="产品目录",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			}
			//		}
			//	},
			//	new MenuItem
			//	{
			//		Name="交易管理",
			//		Children = new[]
			//		{
			//			new MenuItem
			//			{
			//				Name="订单管理",
			//				Children = new[]
			//				{
			//					new MenuItem
			//					{
			//						Name="交易",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="交易项目",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					}
			//				}
			//			},
			//			new MenuItem
			//			{
			//				Name="发货管理",
			//				Children = new[]
			//				{
			//					new MenuItem
			//					{
			//						Name="发货计划",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="发货地址",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="发货渠道",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="发货地址",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="快递查询",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					}
			//				}
			//			},
			//			new MenuItem
			//			{
			//				Name="自动发货管理",
			//				Children = new[]
			//				{
			//					new MenuItem
			//					{
			//						Name="自动发货规格",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="导入自动发货数据",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="自动发货导入记录查询",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="自动发货记录查询",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					}
			//				}
			//			}
			//		}
			//	},
			//	new MenuItem
			//	{
			//		Name="财务管理",
			//		Children=new[]{
			//			new MenuItem
			//			{
			//				Name="账户",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			},
			//			new MenuItem
			//			{
			//				Name="充值记录",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			},
			//			new MenuItem
			//			{
			//				Name="提现记录",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			},
			//			new MenuItem
			//			{
			//				Name="退款记录",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			},
			//			new MenuItem
			//			{
			//				Name="账户科目",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			}
			//		}
			//	},
			//	new MenuItem
			//	{
			//		Name="促销管理",
			//		Children=new[]
			//		{
			//			new MenuItem
			//			{
			//				Name="活动管理",
			//				Children=new[]
			//				{
			//					new MenuItem{
			//						Name="活动计划",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem{
			//						Name="活动参与记录",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem{
			//						Name="活动获奖记录",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},

			//				}
			//			},
			//			new MenuItem
			//			{
			//				Name="专属管理",
			//				Children=new[]
			//				{
			//					new MenuItem{
			//						Name="专属活动记录",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem{
			//						Name="专属活动参与记录",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					}
			//				}
			//			},
			//			new MenuItem
			//			{
			//				Name="优惠券",
			//				Children=new[]
			//				{
			//					new MenuItem{
			//						Name="优惠券计划",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem{
			//						Name="优惠券发放记录",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem{
			//						Name="优惠券使用记录",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem{
			//						Name="优惠券模板",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					}
			//				}
			//			},
			//			new MenuItem
			//			{
			//				Name="实物奖品",
			//				Children=new[]
			//				{
			//					new MenuItem{
			//						Name="实物奖品发放计划",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem{
			//						Name="实物奖品发放记录",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					}
			//				}
			//			}

			//		}
			//	},
			//	new MenuItem
			//	{
			//		Name="客服管理",
			//		Children=new[]
			//		{
			//			new MenuItem
			//			{
			//				Name="用户反馈",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			},

			//			new MenuItem
			//			{
			//				Name="全体通知",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			},
			//			new MenuItem
			//			{
			//				Name="会员通知",
			//				Action=MenuItemAction.Link,
			//				ActionArgument="http://www.sina.com.cn"
			//			}
			//		}

			//	},
			//	new MenuItem
			//	{
			//		Name="内容管理",
			//		Children=new[]
			//		{
			//			new MenuItem
			//			{
			//				Name="PC页面管理",
			//				Children=new[]
			//				{
			//					new MenuItem
			//					{
			//						Name="PC页面头部菜单",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="PC页面头部产品分类",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="PC页面头部幻灯片",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="PC页面尾部菜单",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="PC广告区",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//				}
			//			},
			//			new MenuItem
			//			{
			//				Name="移动端管理",
			//				Children=new[]
			//				{
			//					new MenuItem
			//					{
			//						Name="移动端头部菜单",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="移动端产品分类",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="移动端幻灯片",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					}
			//				}
			//			},
			//			new MenuItem
			//			{
			//				Name="文档管理",
			//				Children=new[]
			//				{
			//					new MenuItem
			//					{
			//						Name="PC系统介绍",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="PC帮助",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="移动端系统介绍",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					},
			//					new MenuItem
			//					{
			//						Name="移动端帮助",
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn"
			//					}
			//				}
			//			},
			//		}
			//	},
			//	new MenuItem
			//	{
			//		Name="系统管理",
			//		Children=new[]
			//		{
			//			new MenuItem
			//			{
			//				Name="业务权限管理",
			//				Children=(await sim.GetServiceMenuItems(sp,BizAdminService))
			//				.Concat(new []
			//				{
			//					new MenuItem
			//					{
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn",
			//						Name="业务管理角色"
			//					},
			//					new MenuItem
			//					{
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn",
			//						Name="业务管理权限"
			//					},
			//					new MenuItem
			//					{
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn",
			//						Name="业务操作日志"
			//					},
			//				})
			//			}
			//		}

			//	}
			//};

			//await sp.NewMenu(null, "bizness", "业务管理后台", bizMenuItems);


			//var sysMenuItems = new[]
			//{

			//	new MenuItem
			//	{
			//		Name="系统管理",
			//		Children=new[]
			//		{

			//			new MenuItem
			//			{
			//				Name="系统权限管理",
			//				Children=(await sim.GetServiceMenuItems(sp,SysAdminService))
			//				.Concat(new []
			//				{
			//					new MenuItem
			//					{
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn",
			//						Name="系统管理角色"
			//					},
			//					new MenuItem
			//					{
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn",
			//						Name="系统管理权限"
			//					},
			//					new MenuItem
			//					{
			//						Action=MenuItemAction.Link,
			//						ActionArgument="http://www.sina.com.cn",
			//						Name="系统操作日志"
			//					},
			//				})
			//			},
			//			new MenuItem
			//			{
			//				Name="其他设置",
			//				Children=await sim.GetServiceMenuItems(sp,MenuService)
			//			},
			//			new MenuItem
			//			{
			//				Name="系统服务管理",
			//				Children=new[]
			//				{
			//					new MenuItem
			//					{
			//						Action=MenuItemAction.EntityManager,
			//						ActionArgument="系统服务实例"
			//					},
			//					new MenuItem
			//					{
			//						Action=MenuItemAction.EntityManager,
			//						ActionArgument="系统服务定义"
			//					},
			//					new MenuItem
			//					{
			//						Action=MenuItemAction.EntityManager,
			//						ActionArgument="系统服务实现"
			//					}
			//				}
			//			}
			//		}

			//	}
			//};

			//await sp.NewMenu(null, "system", "系统管理后台", sysMenuItems);
		}
	}
}
