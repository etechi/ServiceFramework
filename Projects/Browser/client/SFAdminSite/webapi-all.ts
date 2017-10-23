
export interface IQueryPaging {
    offset?: number;
    limit?: number;
    sortMethod?: string;
    sortOrder?: "Asc" | "Desc";

	totalRequired ?: boolean;
	summaryRequired ?: boolean;
}
export interface ICallOptions
{
	paging?: IQueryPaging,
	query?:any,
	serviceId?:number
}
export interface IApiInvoker{
	(type: string,method: string,query: { [index: string]: any},post: { [index: string]: any}, opts?: ICallOptions) :any
}

var _invoker:IApiInvoker=null;
export function setApiInvoker(invoker:IApiInvoker){
	_invoker=invoker;
}


// 
export interface SF$Core$Hosting$FilePathDefination {
	//数据目录
	//类型:string
	DataPath?: string;
	//内容目录
	//类型:string
	ContentPath?: string;
	//临时目录
	//类型:string
	TempPath?: string;
	//配置目录
	//类型:string
	ConfigPath?: string;
}
// 
export interface SF$Core$Hosting$FilePathResolverSettingType {
	//
	//类型:long
	DefaultFilePathDefinationSource?: number;
	//
	//类型:SF.Core.Hosting.FilePathDefination
	Setting: SF$Core$Hosting$FilePathDefination;
}
// 
export interface SF$Core$Caching$LocalFileCacheSetting {
	//缓存根目录
	//类型:string
	RootPath: string;
}
// 
export interface SF$Core$Caching$LocalFileCacheSettingType {
	//
	//类型:SF.Core.Caching.LocalFileCacheSetting
	Setting: SF$Core$Caching$LocalFileCacheSetting;
}
// 
export interface SF$Services$Security$DataProtectorSettingType {
	//
	//类型:string
	GlobalPassword: string;
}
// 
export interface SF$Services$Security$PasswordHasherSettingType {
	//全局密钥
	//类型:string
	GlobalPassword: string;
}
// 站点设置
export interface SF$Services$Settings$AppSiteSetting {
	//网站名称
	//类型:string
	SiteName: string;
	//网站主图标
	//类型:string
	SiteLogo: string;
	//网站小图标
	//类型:string
	SiteIcon: string;
	//网站版权信息
	//类型:string
	SiteCopyright: string;
	//网站认证(ICP证等)
	//类型:string
	SiteCert?: string;
}
// 
export interface SF$Services$Settings$SettingService$SF$Services$Settings$AppSiteSetting$SettingType {
	//
	//类型:SF.Services.Settings.AppSiteSetting
	Value: SF$Services$Settings$AppSiteSetting;
}
// 客服设置
export interface SF$Services$Settings$CustomServiceSetting {
	//客服电话
	//类型:string
	CSPhoneNumber?: string;
	//在线客服链接
	//类型:string
	CSOnlineService?: string;
	//QQ客服号
	//类型:string
	CSQQ?: string;
	//客服微信链接
	//类型:string
	CSWeichatLink?: string;
	//新浪微博链接
	//类型:string
	CSWeiboLink?: string;
}
// 
export interface SF$Services$Settings$SettingService$SF$Services$Settings$CustomServiceSetting$SettingType {
	//
	//类型:SF.Services.Settings.CustomServiceSetting
	Value: SF$Services$Settings$CustomServiceSetting;
}
// 调试设置
export interface SF$Services$Settings$DebugSetting {
	//调试模式
	//类型:bool
	DebugMode?: boolean;
	//调试用户ID
	//类型:int
	DebugUserId: number;
}
// 
export interface SF$Services$Settings$SettingService$SF$Services$Settings$DebugSetting$SettingType {
	//
	//类型:SF.Services.Settings.DebugSetting
	Value: SF$Services$Settings$DebugSetting;
}
// HTTP设置
export interface SF$Services$Settings$HttpSetting {
	//主域名
	//类型:string
	Domain?: string;
	//启用HTTPS模式
	//类型:bool
	HttpsMode?: boolean;
	//HTTP基础路径
	//类型:string
	HttpRoot?: string;
	//资源文件基础路径
	//类型:string
	ResBase?: string;
	//图片资源文件基础路径
	//类型:string
	ImageResBase?: string;
}
// 
export interface SF$Services$Settings$SettingService$SF$Services$Settings$HttpSetting$SettingType {
	//
	//类型:SF.Services.Settings.HttpSetting
	Value: SF$Services$Settings$HttpSetting;
}
// 系统设置
export interface SF$Services$Settings$SystemSetting {
	//系统名称
	//类型:string
	SystemName?: string;
}
// 
export interface SF$Services$Settings$SettingService$SF$Services$Settings$SystemSetting$SettingType {
	//
	//类型:SF.Services.Settings.SystemSetting
	Value: SF$Services$Settings$SystemSetting;
}
// 
export interface SF$Management$FrontEndContents$SiteManagerSettingType {
}
// 
export interface SF$Management$FrontEndContents$SiteTemplateManagerSettingType {
}
// 
export interface SF$Management$FrontEndContents$ContentManagerSettingType {
}
// 
export interface SF$Common$TextMessages$Management$EntityMsgRecordManagerSettingType {
}
// 
export interface SF$Common$TextMessages$SimPhoneTextMessageServiceSettingType {
	//
	//类型:long
	Logger?: number;
	//
	//类型:long
	ServiceInstanceDescriptor?: number;
}
// 
export interface Lazy_IDataSet_SF$Management$MenuServices$Entity$DataModels$MenuItem {
	//
	//类型:bool
	IsValueCreated?: boolean;
}
// 
export interface SF$Management$MenuServices$Entity$EntityMenuService$SF$Management$MenuServices$Entity$DataModels$Menu_SF$Management$MenuServices$Entity$DataModels$MenuItem$SettingType {
	//
	//类型:Lazy_IDataSet_SF.Management.MenuServices.Entity.DataModels.MenuItem
	MenuItemSet: Lazy_IDataSet_SF$Management$MenuServices$Entity$DataModels$MenuItem;
}
// 
export interface SF$Auth$Identities$Entity$EntityIdentityCredentialStorage$SF$Auth$Identities$Entity$DataModels$Identity_SF$Auth$Identities$Entity$DataModels$IdentityCredential$SettingType {
	//
	//类型:long
	TimeService?: number;
	//
	//类型:long
	ServiceInstanceDescriptor?: number;
}
// 
export interface SF$Auth$Identities$Entity$EntityIdentityManagementService$SF$Auth$Identities$Entity$DataModels$Identity_SF$Auth$Identities$Entity$DataModels$IdentityCredential$SettingType {
}
// 
export interface SF$Auth$Identities$IdentityCredentialProviders$ConfirmMessageTemplateSetting {
	//
	//类型:string
	SigninMessageTemplate?: string;
	//
	//类型:string
	SignupMessageTemplate?: string;
	//
	//类型:string
	PasswordRecorveryMessageTemplate?: string;
	//
	//类型:string
	NormalConfirmMessageTemplate?: string;
}
// 
export interface SF$Auth$Identities$IdentityCredentialProviders$PhoneNumberIdentityCredentialProviderSettingType {
	//
	//类型:long
	IdentStorage?: number;
	//
	//类型:long
	PhoneNumberValidator?: number;
	//
	//类型:long
	TextMessageService?: number;
	//
	//类型:SF.Auth.Identities.IdentityCredentialProviders.ConfirmMessageTemplateSetting
	ConfirmMessageSetting: SF$Auth$Identities$IdentityCredentialProviders$ConfirmMessageTemplateSetting;
	//
	//类型:long
	ServiceInstanceMeta?: number;
}
// 
export interface SF$Auth$Identities$IdentityCredentialProviders$UserAccountIdentityCredentialProviderSettingType {
	//
	//类型:long
	IdentStorage?: number;
	//
	//类型:long
	ServiceInstanceMeta?: number;
}
// 
export interface SF$Auth$Identities$IdentityServiceSetting {
	//
	//类型:bool
	VerifyCodeVisible?: boolean;
	//
	//类型:ILocalCache_SF.Auth.Identities.VerifyCode
	VerifyCodeCache?: ILocalCache_SF$Auth$Identities$VerifyCode;
	//
	//类型:long
	IdentStorage?: number;
	//
	//类型:long
	ClientService?: number;
	//
	//类型:long
	DataProtector?: number;
	//
	//类型:long
	PasswordHasher?: number;
	//
	//类型:long
	TimeService?: number;
	//
	//类型:ILocalCache_SF.Auth.Identities.Internals.IdentityData
	IdentityDataCache?: ILocalCache_SF$Auth$Identities$Internals$IdentityData;
	//
	//类型:long
	DefaultIdentityCredentialProvider?: number;
}
// 
export interface ILocalCache_SF$Auth$Identities$VerifyCode {
}
// 
export interface ILocalCache_SF$Auth$Identities$Internals$IdentityData {
}
// 
export interface SF$Auth$Identities$IdentityServiceSettingType {
	//
	//类型:SF.Auth.Identities.IdentityServiceSetting
	Setting: SF$Auth$Identities$IdentityServiceSetting;
}
// 
export interface SF$Users$Members$MemberServiceSettingType {
	//
	//类型:long
	UserManagerService?: number;
	//
	//类型:long
	ServiceInstanceDescriptor?: number;
}
// 
export interface SF$Users$Members$MemberManagementService$SF$Users$Members$DataModels$Member$SettingType {
	//
	//类型:long
	IdentityService?: number;
	//
	//类型:long
	CallPlanProvider?: number;
}
// 
export interface SF$Management$SysAdmins$SysAdminManagementService$SF$Management$SysAdmins$DataModels$SysAdmin$SettingType {
	//
	//类型:long
	IdentityService?: number;
	//
	//类型:long
	CallPlanProvider?: number;
}
// 
export interface SF$Management$SysAdmins$SysAdminServiceSettingType {
	//
	//类型:long
	UserManagerService?: number;
	//
	//类型:long
	ServiceInstanceDescriptor?: number;
}
// 
export interface SF$Management$BizAdmins$Entity$BizAdminManagementService$SF$Management$BizAdmins$DataModels$BizAdmin$SettingType {
	//
	//类型:long
	IdentityService?: number;
	//
	//类型:long
	CallPlanProvider?: number;
}
// 
export interface SF$Management$BizAdmins$BizAdminServiceSettingType {
	//
	//类型:long
	UserManagerService?: number;
	//
	//类型:long
	ServiceInstanceDescriptor?: number;
}
// 
export interface SF$Management$FrontEndContents$Friendly$FriendlyContentSetting {
	//PC头部菜单
	//类型:long
	PCHeadMenuId: number;
	//PC头部产品分类菜单
	//类型:long
	PCHeadProductCategoryMenuId: number;
	//PC首页幻灯片
	//类型:long
	PCHomePageSliderId: number;
	//PC尾部菜单
	//类型:long
	PCHomeTailMenuId: number;
	//PC尾部链接
	//类型:long
	PCHomeTailLinksId: number;
	//PC广告位分类
	//类型:string
	PCAdCategory?: string;
	//移动端引导页图片
	//类型:long
	MobileLandingPageImagesId: number;
	//移动端首页幻灯片
	//类型:long
	MobileHomePageSliderId: number;
	//移动端首页链接菜单
	//类型:long
	MobileHomeIconLinkId: number;
	//移动端产品分类菜单
	//类型:long
	MobileProductCategoryMenuId: number;
	//移动端广告位分类
	//类型:string
	MobileAdCategory?: string;
}
// 
export interface SF$Services$Settings$SettingService$SF$Management$FrontEndContents$Friendly$FriendlyContentSetting$SettingType {
	//
	//类型:SF.Management.FrontEndContents.Friendly.FriendlyContentSetting
	Value: SF$Management$FrontEndContents$Friendly$FriendlyContentSetting;
}
// 
export interface SF$Common$Documents$Management$DocumentCategoryManagerSettingType {
	//
	//类型:long
	DataSetAutoEntityProviderFactory?: number;
}
// 
export interface SF$Common$Documents$Management$DocumentManagerSettingType {
	//
	//类型:long
	DataSetAutoEntityProviderFactory?: number;
}
// 
export interface Lazy_IDataSet_SF$Common$Documents$DataModels$Document {
	//
	//类型:bool
	IsValueCreated?: boolean;
}
// 
export interface Lazy_IDataSet_SF$Common$Documents$DataModels$DocumentCategory {
	//
	//类型:bool
	IsValueCreated?: boolean;
}
// 
export interface SF$Common$Documents$DocumentServiceSettingType {
	//
	//类型:Lazy_IDataSet_SF.Common.Documents.DataModels.Document
	Documents: Lazy_IDataSet_SF$Common$Documents$DataModels$Document;
	//
	//类型:Lazy_IDataSet_SF.Common.Documents.DataModels.DocumentCategory
	Categories: Lazy_IDataSet_SF$Common$Documents$DataModels$DocumentCategory;
	//
	//类型:long
	ServiceInstanceDescriptor?: number;
}
// 
export interface SF$Biz$Products$Entity$ProductManagerSettingType {
	//
	//类型:long
	ItemNotifier?: number;
}
// 
export interface SF$Biz$Products$Entity$ProductTypeManagerSettingType {
}
// 
export interface SF$Biz$Products$Entity$CategoryManagerSettingType {
	//
	//类型:long
	Notifier?: number;
}
// 
export interface SF$Biz$Products$Entity$ItemManagerSettingType {
	//
	//类型:long
	ItemNotifier?: number;
}
// 
export interface SF$Users$Promotions$MemberInvitations$Entity$EntityMemberInvitationManagementServiceSettingType {
	//
	//类型:long
	DataSetAutoEntityProviderFactory?: number;
}
// 
export interface SF$Services$Media$MediaManagerSettingType {
	//
	//类型:long
	MetaCache?: number;
}
// 媒体WebApi配置
export interface SF$Services$Media$MediaServiceSetting {
	//上传文件类型标识
	//类型:string
	UploadMediaType: string;
	//最大文件尺寸(MB)
	//类型:int
	MaxSize: number;
	//总是转为Jpeg图片
	//类型:bool
	ConvertToJpeg?: boolean;
	//图片最大尺寸
	//类型:int
	MaxImageSize: number;
	//支持文件类型
	//类型:string[]
	Mimes?: string[];
	//允许任意图片样式
	//类型:bool
	SupportedAllFormats?: boolean;
	//支持图片处理样式
	//类型:string[]
	SupportedFormats?: string[];
	//缓存的媒体资源类型
	//类型:string[]
	MediaCacheTypes?: string[];
}
// 
export interface SF$Services$Media$MediaServiceSettingType {
	//媒体管理器
	//类型:long
	Manager?: number;
	//服务设置
	//类型:SF.Services.Media.MediaServiceSetting
	Setting: SF$Services$Media$MediaServiceSetting;
	//MIME服务
	//类型:long
	MimeResolver?: number;
	//
	//类型:long
	FileCollection?: number;
	//
	//类型:long
	InvokeContext?: number;
	//
	//类型:long
	ImageProvider?: number;
	//文件缓存
	//类型:long
	FileCache?: number;
}
// 
export interface SF$Services$Media$Storages$FileSystemMediaStorageSettingType {
	//
	//类型:long
	MimeResolver?: number;
	//文件根目录
	//类型:string
	RootPath: string;
	//路径解析器
	//类型:long
	PathResolver?: number;
}
// 
export interface SF$Services$Media$Storages$StaticFileMediaStorageSettingType {
	//
	//类型:long
	MimeResolver?: number;
	//文件根目录
	//类型:string
	RootPath: string;
	//路径解析器
	//类型:long
	PathResolver?: number;
}
// 
export interface Hygou$HygouSetting {
	//PC站点默认帮助文档
	//类型:long
	PCHelpCenterDefaultDocId: number;
	//主产品目录
	//类型:long
	MainProductCategoryId: number;
	//默认卖家
	//类型:long
	DefaultSellerId: number;
}
// 
export interface SF$Services$Settings$SettingService$Hygou$HygouSetting$SettingType {
	//
	//类型:Hygou.HygouSetting
	Value: Hygou$HygouSetting;
}
// 
export interface ObjectKey_string {
	//标识
	//类型:string
	Id?: string;
}
// 
export interface SF$Core$ServiceManagement$Models$ServiceDeclaration {
	//ID
	//类型:string
	Id?: string;
	//类型
	//类型:string
	Type?: string;
	//名称
	//类型:string
	Name?: string;
	//描述
	//类型:string
	Description?: string;
	//分组
	//类型:string
	Group?: string;
	//是否禁用
	//类型:bool
	Disabled?: boolean;
}
// 
export interface SF$Core$ServiceManagement$Management$ServiceDeclarationQueryArgument {
	//ID
	//类型:ObjectKey_string
	Id?: ObjectKey_string;
	//服务定义名称
	//类型:string
	Name?: string;
	//服务定义分类
	//类型:string
	Group?: string;
}
// 
export interface QueryResult_SF$Core$ServiceManagement$Models$ServiceDeclaration {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Core.ServiceManagement.Models.ServiceDeclaration[]
	Items?: SF$Core$ServiceManagement$Models$ServiceDeclaration[];
}
// 
export interface SF$Entities$ISummary {
}
// 
export interface QueryResult_ObjectKey_string {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:ObjectKey_string[]
	Items?: ObjectKey_string[];
}
// 
export interface SF$Core$ServiceManagement$Models$ServiceImplement {
	//ID
	//类型:string
	Id?: string;
	//名称
	//类型:string
	Type?: string;
	//名称
	//类型:string
	Name?: string;
	//描述
	//类型:string
	Description?: string;
	//分组
	//类型:string
	Group?: string;
	//是否禁用
	//类型:bool
	Disabled?: boolean;
	//服务定义
	//类型:string
	DeclarationId?: string;
	//服务定义
	//类型:string
	DeclarationName?: string;
}
// 
export interface SF$Core$ServiceManagement$Management$ServiceImplementQueryArgument {
	//ID
	//类型:ObjectKey_string
	Id?: ObjectKey_string;
	//服务实现名称
	//类型:string
	Name?: string;
	//服务实现分类
	//类型:string
	Group?: string;
	//服务定义
	//类型:string
	DeclarationId?: string;
}
// 
export interface QueryResult_SF$Core$ServiceManagement$Models$ServiceImplement {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Core.ServiceManagement.Models.ServiceImplement[]
	Items?: SF$Core$ServiceManagement$Models$ServiceImplement[];
}
// 
export interface ObjectKey_long {
	//标识
	//类型:long
	Id: number;
}
// 
export interface SF$Core$ServiceManagement$Models$ServiceInstanceEditable extends SF$Core$ServiceManagement$Models$ServiceInstanceInternal {
	//服务设置
	//类型:string
	Setting?: string;
	//
	//类型:string
	SettingType?: string;
}
// 
export interface SF$Core$ServiceManagement$Models$ServiceInstanceInternal extends SF$Core$ServiceManagement$Models$ServiceInstance {
	//服务定义
	//类型:string
	ServiceId: string;
	//服务实现类型
	//类型:string
	ServiceType: string;
	//服务名称
	//类型:string
	ServiceName?: string;
	//服务实现ID
	//类型:string
	ImplementId: string;
	//服务实现类型
	//类型:string
	ImplementType: string;
	//服务实现名称
	//类型:string
	ImplementName?: string;
	//父服务实例
	//类型:long
	ContainerId?: number;
	//父服务
	//类型:string
	ContainerName?: string;
	//
	//类型:SF.Core.ServiceManagement.Models.ServiceInstanceInternal[]
	Children?: SF$Core$ServiceManagement$Models$ServiceInstanceInternal[];
}
// 
export interface SF$Core$ServiceManagement$Models$ServiceInstance extends UIObjectEntityBase_long {
	//服务标识
	//类型:string
	ServiceIdent?: string;
	//优先级
	//类型:int
	ItemOrder: number;
}
// 
export interface UIObjectEntityBase_long extends ObjectEntityBase_long {
	//标题
	//类型:string
	Title: string;
	//副标题
	//类型:string
	SubTitle?: string;
	//提示
	//类型:string
	Remarks?: string;
	//说明
	//类型:string
	Description?: string;
	//图片
	//类型:string
	Image?: string;
	//图标
	//类型:string
	Icon?: string;
}
// 
export interface ObjectEntityBase_long {
	//Id
	//类型:long
	Id: number;
	//名称
	//类型:string
	Name: string;
	//对象状态
	//类型:SF.Entities.EntityLogicState
	LogicState: SF$Entities$EntityLogicState;
	//创建时间
	//类型:datetime
	CreatedTime: string;
	//修改时间
	//类型:datetime
	UpdatedTime: string;
	//内部备注
	//类型:string
	InternalRemarks?: string;
}
// 
export type SF$Entities$EntityLogicState = 'Enabled'|'Disabled'|'Deleted';
export const SF$Entities$EntityLogicStateNames={
	"Enabled":"有效",
	"Disabled":"无效",
	"Deleted":"已删除",
}
// 
export interface SF$Core$ServiceManagement$Management$ServiceInstanceQueryArgument {
	//ID
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//服务实例名称
	//类型:string
	Name?: string;
	//服务定义
	//类型:string
	ServiceId?: string;
	//服务类型
	//类型:string
	ServiceType?: string;
	//服务实现
	//类型:string
	ImplementId?: string;
	//服务类型类型
	//类型:string
	ImplementType?: string;
	//父服务实现
	//类型:long
	ContainerId?: number;
	//服务标识
	//类型:string
	ServiceIdent?: string;
	//是否为默认服务实例
	//类型:bool
	IsDefaultService?: boolean;
}
// 
export interface QueryResult_SF$Core$ServiceManagement$Models$ServiceInstanceInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Core.ServiceManagement.Models.ServiceInstanceInternal[]
	Items?: SF$Core$ServiceManagement$Models$ServiceInstanceInternal[];
}
// 
export interface QueryResult_ObjectKey_long {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:ObjectKey_long[]
	Items?: ObjectKey_long[];
}
// 
export interface SF$Management$FrontEndContents$Site {
	//ID
	//类型:string
	Id?: string;
	//名称
	//类型:string
	Name: string;
	//站点模板
	//类型:long
	TemplateId: number;
	//站点模板
	//类型:string
	TemplateName?: string;
}
// 
export interface QueryArgument_ObjectKey_string {
	//ID
	//类型:ObjectKey_string
	Id?: ObjectKey_string;
}
// 
export interface QueryResult_SF$Management$FrontEndContents$Site {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Management.FrontEndContents.Site[]
	Items?: SF$Management$FrontEndContents$Site[];
}
// 
export interface SF$Management$FrontEndContents$SiteTemplate {
	//ID
	//类型:long
	Id: number;
	//名称
	//类型:string
	Name: string;
	//
	//类型:SF.Management.FrontEndContents.SiteConfigModels.SiteModel
	Model: SF$Management$FrontEndContents$SiteConfigModels$SiteModel;
}
// 
export interface SF$Management$FrontEndContents$SiteConfigModels$SiteModel {
	//名称
	//类型:string
	name?: string;
	//页面列表
	//类型:SF.Management.FrontEndContents.SiteConfigModels.PageModel[]
	pages?: SF$Management$FrontEndContents$SiteConfigModels$PageModel[];
}
// 
export interface SF$Management$FrontEndContents$SiteConfigModels$PageModel {
	//页面标示
	//类型:string
	ident: string;
	//页面名称
	//类型:string
	name: string;
	//页面备注
	//类型:string
	remarks?: string;
	//是否禁用
	//类型:bool
	disabled?: boolean;
	//包含页面内容
	//类型:string[]
	includes?: string[];
	//页面块
	//类型:SF.Management.FrontEndContents.SiteConfigModels.BlockModel[]
	blocks?: SF$Management$FrontEndContents$SiteConfigModels$BlockModel[];
}
// 
export interface SF$Management$FrontEndContents$SiteConfigModels$BlockModel {
	//页面块标示
	//类型:string
	ident: string;
	//页面块名称
	//类型:string
	name?: string;
	//页面块备注
	//类型:string
	remarks?: string;
	//是否禁用
	//类型:bool
	disabled?: boolean;
	//块内容
	//类型:SF.Management.FrontEndContents.SiteConfigModels.BlockContentModel[]
	contents?: SF$Management$FrontEndContents$SiteConfigModels$BlockContentModel[];
}
// 
export interface SF$Management$FrontEndContents$SiteConfigModels$BlockContentModel {
	//名称
	//类型:string
	name?: string;
	//显示内容
	//类型:long
	content?: number;
	//内容配置
	//类型:string
	contentConfig?: string;
	//视图引擎
	//类型:string
	render: string;
	//视图
	//类型:string
	view: string;
	//视图配置
	//类型:string
	viewConfig?: string;
	//内容块图片
	//类型:string
	image?: string;
	//内容块图标
	//类型:string
	icon?: string;
	//字体图标类
	//类型:string
	fontIcon?: string;
	//链接
	//类型:string
	uri?: string;
	//主标题
	//类型:string
	title1?: string;
	//次标题1
	//类型:string
	title2?: string;
	//次标题2
	//类型:string
	title3?: string;
	//摘要
	//类型:string
	summary?: string;
	//是否禁用
	//类型:bool
	disabled?: boolean;
}
// 
export interface SF$Management$FrontEndContents$SiteTemplateQueryArgument {
	//
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Management$FrontEndContents$SiteTemplate {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Management.FrontEndContents.SiteTemplate[]
	Items?: SF$Management$FrontEndContents$SiteTemplate[];
}
// 
export interface SF$Management$FrontEndContents$Content extends SF$Management$FrontEndContents$ContentItem {
	//ID
	//类型:long
	Id: number;
	//内容分类
	//类型:string
	Category: string;
	//内容名称
	//类型:string
	Name: string;
	//提供者类型
	//类型:string
	ProviderType?: string;
	//提供者配置
	//类型:string
	ProviderConfig?: string;
	//是否禁用
	//类型:bool
	Disabled?: boolean;
}
// 
export interface SF$Management$FrontEndContents$ContentItem {
	//大图
	//类型:string
	Image?: string;
	//图标
	//类型:string
	Icon?: string;
	//字体图标类
	//类型:string
	FontIcon?: string;
	//主标题
	//类型:string
	Title1?: string;
	//次标题1
	//类型:string
	Title2?: string;
	//次标题2
	//类型:string
	Title3?: string;
	//摘要
	//类型:string
	Summary?: string;
	//链接
	//类型:string
	Uri?: string;
	//链接目标
	//类型:string
	UriTarget?: string;
	//子项目
	//类型:SF.Management.FrontEndContents.ContentItem[]
	Items?: SF$Management$FrontEndContents$ContentItem[];
}
// 
export interface SF$Management$FrontEndContents$ContentQueryArgument {
	//
	//类型:long
	Id?: number;
	//
	//类型:string
	Category?: string;
	//
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Management$FrontEndContents$Content {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Management.FrontEndContents.Content[]
	Items?: SF$Management$FrontEndContents$Content[];
}
// 
export interface SF$Management$FrontEndContents$IContent extends SF$Management$FrontEndContents$IContentItem {
	//
	//类型:long
	Id: number;
	//
	//类型:string
	Name?: string;
	//
	//类型:string
	Category?: string;
	//
	//类型:string
	ProviderType?: string;
	//
	//类型:string
	ProviderConfig?: string;
	//
	//类型:bool
	Disabled?: boolean;
}
// 
export interface SF$Management$FrontEndContents$IContentItem {
	//
	//类型:string
	Image?: string;
	//
	//类型:string
	Icon?: string;
	//
	//类型:string
	FontIcon?: string;
	//
	//类型:string
	Uri?: string;
	//
	//类型:string
	UriTarget?: string;
	//
	//类型:string
	Title1?: string;
	//
	//类型:string
	Title2?: string;
	//
	//类型:string
	Title3?: string;
	//
	//类型:string
	Summary?: string;
	//
	//类型:SF.Management.FrontEndContents.ContentItem[]
	Items?: SF$Management$FrontEndContents$ContentItem[];
}
// 
export interface SF$Common$TextMessages$Management$MsgRecord extends SF$Data$Models$EventEntityBase {
	//状态
	//类型:SF.Common.TextMessages.Management.SendStatus
	Status: SF$Common$TextMessages$Management$SendStatus;
	//接收方
	//类型:string
	Target?: string;
	//发送服务
	//类型:long
	ServiceId: number;
	//发送服务
	//类型:string
	ServiceName?: string;
	//发送方
	//类型:string
	Sender?: string;
	//标题
	//类型:string
	Title?: string;
	//内容
	//类型:string
	Body?: string;
	//消息头部
	//类型:string
	Headers?: string;
	//消息参数
	//类型:string
	Args?: string;
	//完成时间
	//类型:datetime
	CompletedTime?: string;
	//错误信息
	//类型:string
	Error?: string;
	//发送结果
	//类型:string
	Result?: string;
	//跟踪对象
	//类型:string
	TrackEntityId?: string;
}
// 
export interface SF$Data$Models$EventEntityBase extends EventEntityBase_long {
}
// 
export interface EventEntityBase_long {
	//Id
	//类型:long
	Id: number;
	//时间
	//类型:datetime
	Time: string;
	//用户
	//类型:long
	UserId?: number;
	//用户
	//类型:string
	UserName?: string;
}
// 
export type SF$Common$TextMessages$Management$SendStatus = 'Sending'|'Completed'|'Failed';
export const SF$Common$TextMessages$Management$SendStatusNames={
	"Sending":"Sending",
	"Completed":"Completed",
	"Failed":"Failed",
}
// 
export interface SF$Common$TextMessages$Management$MsgRecordQueryArgument extends QueryArgument_ObjectKey_long {
	//状态
	//类型:SF.Common.TextMessages.Management.SendStatus
	Status?: SF$Common$TextMessages$Management$SendStatus;
	//目标用户
	//类型:long
	TargeUserId?: number;
	//发送服务
	//类型:long
	ServiceId?: number;
	//发送时间
	//类型:QueryRange_datetime
	Time?: QueryRange_datetime;
	//发送对象
	//类型:string
	Target?: string;
}
// 
export interface QueryArgument_ObjectKey_long {
	//ID
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
}
// 
export interface QueryRange_datetime {
	//
	//类型:datetime
	Begin?: string;
	//
	//类型:datetime
	End?: string;
}
// 
export interface QueryResult_SF$Common$TextMessages$Management$MsgRecord {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.TextMessages.Management.MsgRecord[]
	Items?: SF$Common$TextMessages$Management$MsgRecord[];
}
// 
export interface SF$Management$MenuServices$MenuItem extends UIObjectEntityBase_long {
	//字体图标
	//类型:string
	FontIcon?: string;
	//动作
	//类型:SF.Management.MenuServices.MenuActionType
	Action: SF$Management$MenuServices$MenuActionType;
	//动作参数
	//类型:string
	ActionArgument?: string;
	//服务
	//类型:long
	ServiceId?: number;
	//子菜单
	//类型:SF.Management.MenuServices.MenuItem[]
	Children?: SF$Management$MenuServices$MenuItem[];
	//
	//类型:long
	ParentId?: number;
}
// 
export type SF$Management$MenuServices$MenuActionType = 'None'|'EntityManager'|'Form'|'List'|'IFrame'|'Link';
export const SF$Management$MenuServices$MenuActionTypeNames={
	"None":"无",
	"EntityManager":"实体管理",
	"Form":"显示表单",
	"List":"显示列表",
	"IFrame":"显示内嵌网页",
	"Link":"打开链接",
}
// 
export interface SF$Management$MenuServices$Models$Menu extends ObjectEntityBase_long {
	//菜单引用标识
	//类型:string
	Ident: string;
}
// 
export interface SF$Management$MenuServices$MenuQueryArgument {
	//
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//名称
	//类型:string
	Name?: string;
	//标识
	//类型:string
	Ident?: string;
}
// 
export interface QueryResult_SF$Management$MenuServices$Models$Menu {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Management.MenuServices.Models.Menu[]
	Items?: SF$Management$MenuServices$Models$Menu[];
}
// 
export interface SF$Management$MenuServices$Models$MenuEditable extends SF$Management$MenuServices$Models$Menu {
	//菜单项
	//类型:SF.Management.MenuServices.MenuItem[]
	Items?: SF$Management$MenuServices$MenuItem[];
}
// 身份标识
export interface SF$Auth$Identities$Models$IdentityEditable extends SF$Auth$Identities$Models$IdentityInternal {
	//
	//类型:string
	PasswordHash?: string;
	//
	//类型:string
	SecurityStamp?: string;
	//
	//类型:SF.Auth.Identities.Models.IdentityCredential[]
	Credentials?: SF$Auth$Identities$Models$IdentityCredential[];
	//注册附加参数
	//类型:string
	SignupExtraArgument?: string;
}
// 身份标识
export interface SF$Auth$Identities$Models$IdentityInternal extends SF$Auth$Identities$Models$Identity {
	//创建标识
	//类型:string
	CreateCredential?: string;
	//创建标识提供者
	//类型:long
	CreateCredentialProviderId: number;
	//创建时间
	//类型:datetime
	CreateTime: string;
	//更新时间
	//类型:datetime
	UpdateTime: string;
	//对象状态
	//类型:SF.Entities.EntityLogicState
	LogicState: SF$Entities$EntityLogicState;
}
// 身份标识
export interface SF$Auth$Identities$Models$Identity {
	//ID
	//类型:long
	Id: number;
	//名称
	//类型:string
	Name: string;
	//图标
	//类型:string
	Icon?: string;
	//所属对象
	//类型:string
	OwnerId: string;
}
// 
export interface SF$Auth$Identities$Models$IdentityCredential {
	//
	//类型:long
	ProviderId: number;
	//
	//类型:long
	IdentityId: number;
	//
	//类型:string
	Credential?: string;
	//
	//类型:string
	UnionIdent?: string;
	//
	//类型:datetime
	BindTime: string;
	//
	//类型:datetime
	ConfirmedTime?: string;
}
// 
export interface SF$Auth$Identities$IdentityQueryArgument {
	//
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//
	//类型:string
	Ident?: string;
	//
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Auth$Identities$Models$IdentityInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Auth.Identities.Models.IdentityInternal[]
	Items?: SF$Auth$Identities$Models$IdentityInternal[];
}
// 
export interface SF$Auth$Identities$SigninArgument {
	//身份标识
	//类型:string
	Credential?: string;
	//密码
	//类型:string
	Password?: string;
	//过期时间
	//类型:int
	Expires?: number;
	//人工操作验证码
	//类型:string
	CaptchaCode?: string;
	//是否返回身份令牌
	//类型:bool
	ReturnToken?: boolean;
}
// 
export interface SF$Auth$Identities$SendPasswordRecorveryCodeArgument {
	//身份验证服务ID
	//类型:long
	CredentialProviderId?: number;
	//人工操作验证码
	//类型:string
	CaptchaCode?: string;
	//身份标识
	//类型:string
	Credential?: string;
}
// 
export interface SF$Auth$Identities$ResetPasswordByRecorveryCodeArgument {
	//身份验证服务ID
	//类型:long
	CredentialProviderId?: number;
	//身份标识
	//类型:string
	Credential?: string;
	//验证码
	//类型:string
	VerifyCode?: string;
	//新密码
	//类型:string
	NewPassword?: string;
	//是否返回身份令牌
	//类型:bool
	ReturnToken?: boolean;
}
// 
export interface SF$Auth$Identities$SetPasswordArgument {
	//就密码
	//类型:string
	OldPassword?: string;
	//新密码
	//类型:string
	NewPassword?: string;
	//是否返回身份令牌
	//类型:bool
	ReturnToken?: boolean;
}
// 
export interface SF$Auth$Identities$SendCreateIdentityVerifyCodeArgument {
	//身份验证服务ID
	//类型:long
	CredentialProviderId?: number;
	//人工操作验证码
	//类型:string
	Credetial?: string;
	//人工操作验证码
	//类型:string
	CaptchaCode?: string;
}
// 
export interface SF$Auth$Identities$CreateIdentityArgument {
	//身份验证服务ID
	//类型:long
	CredentialProviderId?: number;
	//身份信息
	//类型:SF.Auth.Identities.Models.Identity
	Identity?: SF$Auth$Identities$Models$Identity;
	//身份标识
	//类型:string
	Credential?: string;
	//密码
	//类型:string
	Password?: string;
	//人工操作验证码
	//类型:string
	CaptchaCode?: string;
	//验证码
	//类型:string
	VerifyCode?: string;
	//是否返回身份令牌
	//类型:bool
	ReturnToken?: boolean;
	//过期时间
	//类型:int
	Expires?: number;
	//附加参数
	//类型:string{}
	ExtraArgument?: {[index:string]:string};
}
// 
export interface SF$Auth$Users$Models$UserDesc {
	//ID
	//类型:long
	Id: number;
	//名称
	//类型:string
	Name?: string;
	//图标
	//类型:string
	Icon?: string;
}
// 
export interface SF$Users$Members$Models$MemberInternal extends SF$Auth$Users$Models$UserInternal {
}
// 
export interface SF$Auth$Users$Models$UserInternal extends SF$Data$Models$ObjectEntityBase {
	//账户名
	//类型:string
	AccountName: string;
}
// 
export interface SF$Data$Models$ObjectEntityBase extends ObjectEntityBase_long {
}
// 
export interface SF$Users$Members$MemberQueryArgument extends SF$Auth$Users$UserQueryArgument {
}
// 
export interface SF$Auth$Users$UserQueryArgument {
	//Id
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//名称
	//类型:string
	Name?: string;
	//账户名
	//类型:string
	AccountName?: string;
}
// 
export interface QueryResult_SF$Users$Members$Models$MemberInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Users.Members.Models.MemberInternal[]
	Items?: SF$Users$Members$Models$MemberInternal[];
}
// 
export interface SF$Users$Members$Models$MemberEditable extends SF$Auth$Users$Models$UserEditable {
}
// 
export interface SF$Auth$Users$Models$UserEditable extends SF$Auth$Users$Models$UserInternal {
	//图标
	//类型:string
	Icon?: string;
	//密码
	//类型:string
	Password?: string;
}
// 
export interface SF$Management$SysAdmins$Models$SysAdminInternal extends SF$Auth$Users$Models$UserInternal {
	//账号
	//类型:string
	Account: string;
}
// 
export interface SF$Management$SysAdmins$SysAdminQueryArgument extends SF$Auth$Users$UserQueryArgument {
	//
	//类型:string
	Account?: string;
}
// 
export interface QueryResult_SF$Management$SysAdmins$Models$SysAdminInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Management.SysAdmins.Models.SysAdminInternal[]
	Items?: SF$Management$SysAdmins$Models$SysAdminInternal[];
}
// 
export interface SF$Management$SysAdmins$Models$SysAdminEditable extends SF$Auth$Users$Models$UserEditable {
}
// 
export interface SF$Management$BizAdmins$Models$BizAdminInternal extends SF$Auth$Users$Models$UserInternal {
	//账号
	//类型:string
	Account: string;
}
// 
export interface SF$Management$BizAdmins$BizAdminQueryArgument extends SF$Auth$Users$UserQueryArgument {
}
// 
export interface QueryResult_SF$Management$BizAdmins$Models$BizAdminInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Management.BizAdmins.Models.BizAdminInternal[]
	Items?: SF$Management$BizAdmins$Models$BizAdminInternal[];
}
// 
export interface SF$Management$BizAdmins$Models$BizAdminEditable extends SF$Auth$Users$Models$UserEditable {
}
// 
export interface SF$Common$Documents$Management$CategoryInternal extends UITreeContainerEntityBase_SF$Common$Documents$Management$CategoryInternal_SF$Common$Documents$Management$DocumentInternal {
	//父分类
	//类型:long
	ContainerId?: number;
	//父分类
	//类型:string
	ContainerName?: string;
}
// 
export interface UITreeContainerEntityBase_SF$Common$Documents$Management$CategoryInternal_SF$Common$Documents$Management$DocumentInternal extends UITreeContainerEntityBase_SF$Common$Documents$Management$CategoryInternal_long_Nullable_long_SF$Common$Documents$Management$DocumentInternal_Nullable_long {
}
// 
export interface UITreeContainerEntityBase_SF$Common$Documents$Management$CategoryInternal_long_Nullable_long_SF$Common$Documents$Management$DocumentInternal_Nullable_long extends UITreeNodeEntityBase_SF$Common$Documents$Management$CategoryInternal_long_Nullable_long {
	//项目
	//类型:SF.Common.Documents.Management.DocumentInternal[]
	Items?: SF$Common$Documents$Management$DocumentInternal[];
}
// 
export interface UITreeNodeEntityBase_SF$Common$Documents$Management$CategoryInternal_long_Nullable_long extends UIItemEntityBase_long_Nullable_long_SF$Common$Documents$Management$CategoryInternal {
	//子项
	//类型:SF.Common.Documents.Management.CategoryInternal[]
	Children?: SF$Common$Documents$Management$CategoryInternal[];
}
// 
export interface UIItemEntityBase_long_Nullable_long_SF$Common$Documents$Management$CategoryInternal extends UIObjectEntityBase_long {
	//容器ID
	//类型:long
	ContainerId?: number;
	//容器
	//类型:string
	ContainerName?: string;
	//容器
	//类型:SF.Common.Documents.Management.CategoryInternal
	Container?: SF$Common$Documents$Management$CategoryInternal;
	//排位
	//类型:int
	ItemOrder?: number;
}
// 
export interface SF$Common$Documents$Management$DocumentInternal extends SF$Common$Documents$Management$DocumentBase {
}
// 
export interface SF$Common$Documents$Management$DocumentBase extends UIItemEntityBase_SF$Common$Documents$Management$CategoryInternal {
	//文档分类
	//类型:long
	ContainerId?: number;
	//访问标示
	//类型:string
	Ident?: string;
	//发布时间
	//类型:datetime
	PublishDate?: string;
	//
	//类型:string
	ContainerName?: string;
}
// 
export interface UIItemEntityBase_SF$Common$Documents$Management$CategoryInternal extends UIItemEntityBase_long_Nullable_long_SF$Common$Documents$Management$CategoryInternal {
}
// 
export interface SF$Common$Documents$Management$DocumentCategoryQueryArgument extends SF$Entities$QueryArgument {
	//父分类
	//类型:int
	ParentId?: number;
	//名称
	//类型:string
	Name?: string;
}
// 
export interface SF$Entities$QueryArgument extends QueryArgument_ObjectKey_long {
}
// 
export interface QueryResult_SF$Common$Documents$Management$CategoryInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.Documents.Management.CategoryInternal[]
	Items?: SF$Common$Documents$Management$CategoryInternal[];
}
// 
export interface SF$Common$Documents$Management$DocumentQueryArguments extends SF$Entities$QueryArgument {
	//文档分类
	//类型:int
	CategoryId?: number;
	//标题
	//类型:string
	Name?: string;
	//发布日期
	//类型:SF.Entities.NullableDateQueryRange
	PublishDate?: SF$Entities$NullableDateQueryRange;
}
// 
export interface SF$Entities$NullableDateQueryRange extends NullableQueryRange_datetime {
	//
	//类型:datetime
	End?: string;
}
// 
export interface NullableQueryRange_datetime {
	//
	//类型:bool
	NotNull?: boolean;
	//
	//类型:datetime
	Begin?: string;
	//
	//类型:datetime
	End?: string;
}
// 
export interface QueryResult_SF$Common$Documents$Management$DocumentInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.Documents.Management.DocumentInternal[]
	Items?: SF$Common$Documents$Management$DocumentInternal[];
}
// 
export interface SF$Common$Documents$Management$DocumentEditable extends SF$Common$Documents$Management$DocumentBase {
	//
	//类型:string
	Content?: string;
}
// 
export interface SF$Common$Documents$Document extends UIItemEntityBase_SF$Common$Documents$Category {
	//
	//类型:string
	Content?: string;
}
// 
export interface UIItemEntityBase_SF$Common$Documents$Category extends UIItemEntityBase_long_Nullable_long_SF$Common$Documents$Category {
}
// 
export interface UIItemEntityBase_long_Nullable_long_SF$Common$Documents$Category extends UIObjectEntityBase_long {
	//容器ID
	//类型:long
	ContainerId?: number;
	//容器
	//类型:string
	ContainerName?: string;
	//容器
	//类型:SF.Common.Documents.Category
	Container?: SF$Common$Documents$Category;
	//排位
	//类型:int
	ItemOrder?: number;
}
// 
export interface SF$Common$Documents$Category extends UITreeContainerEntityBase_SF$Common$Documents$Category_SF$Common$Documents$Document {
}
// 
export interface UITreeContainerEntityBase_SF$Common$Documents$Category_SF$Common$Documents$Document extends UITreeContainerEntityBase_SF$Common$Documents$Category_long_Nullable_long_SF$Common$Documents$Document_Nullable_long {
}
// 
export interface UITreeContainerEntityBase_SF$Common$Documents$Category_long_Nullable_long_SF$Common$Documents$Document_Nullable_long extends UITreeNodeEntityBase_SF$Common$Documents$Category_long_Nullable_long {
	//项目
	//类型:SF.Common.Documents.Document[]
	Items?: SF$Common$Documents$Document[];
}
// 
export interface UITreeNodeEntityBase_SF$Common$Documents$Category_long_Nullable_long extends UIItemEntityBase_long_Nullable_long_SF$Common$Documents$Category {
	//子项
	//类型:SF.Common.Documents.Category[]
	Children?: SF$Common$Documents$Category[];
}
// 
export interface QueryResult_SF$Common$Documents$Document {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.Documents.Document[]
	Items?: SF$Common$Documents$Document[];
}
// 
export interface QueryResult_SF$Common$Documents$Category {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.Documents.Category[]
	Items?: SF$Common$Documents$Category[];
}
// 
export interface SF$Biz$Products$ProductSpec {
	//ID
	//类型:long
	Id: number;
	//名称
	//类型:string
	Name: string;
	//图片
	//类型:string
	Image?: string;
	//描述
	//类型:string
	Desc?: string;
	//自动发货规格
	//类型:long
	VIADSpecId?: number;
}
// 
export interface SF$Biz$Products$ProductEditable extends SF$Biz$Products$ProductBase {
	//产品类型
	//类型:long
	TypeId: number;
	//产品提供人
	//类型:long
	OwnerUserId: number;
	//商品分类
	//类型:long[]
	CategoryIds?: number[];
	//
	//类型:SF.Biz.Products.ProductContent
	Content: SF$Biz$Products$ProductContent;
	//产品规格
	//类型:SF.Biz.Products.ProductSpecDetail[]
	Specs?: SF$Biz$Products$ProductSpecDetail[];
	//自动发货规格
	//类型:long
	VIADSpecId?: number;
}
// 
export interface SF$Biz$Products$ProductBase {
	//ID
	//类型:long
	Id: number;
	//产品名称
	//类型:string
	Name: string;
	//展示标题
	//类型:string
	Title: string;
	//市场价
	//类型:decimal
	MarketPrice: number;
	//售价
	//类型:decimal
	Price: number;
	//主图
	//类型:string
	Image: string;
	//虚拟商品
	//类型:bool
	IsVirtual?: boolean;
	//禁止优惠券
	//类型:bool
	CouponDisabled?: boolean;
	//产品发布时间
	//类型:datetime
	PublishedTime?: string;
	//产品状态
	//类型:SF.Entities.EntityLogicState
	ObjectState: SF$Entities$EntityLogicState;
}
// 
export interface SF$Biz$Products$ProductContent {
	//产品图片
	//类型:SF.Biz.Products.ProductImage[]
	Images: SF$Biz$Products$ProductImage[];
	//产品介绍
	//类型:SF.Biz.Products.ProductDescItem[]
	Descs: SF$Biz$Products$ProductDescItem[];
}
// 
export interface SF$Biz$Products$ProductImage {
	//图片
	//类型:string
	Image: string;
	//标题
	//类型:string
	Title?: string;
}
// 
export interface SF$Biz$Products$ProductDescItem {
	//
	//类型:string
	Image: string;
	//
	//类型:string
	Title?: string;
}
// 
export interface SF$Biz$Products$ProductSpecDetail extends SF$Biz$Products$ProductSpec {
	//
	//类型:long
	Order: number;
	//状态
	//类型:SF.Entities.EntityLogicState
	ObjectState: SF$Entities$EntityLogicState;
	//创建时间
	//类型:datetime
	CreatedTime: string;
	//修改时间
	//类型:datetime
	UpdatedTime: string;
}
// 
export interface SF$Biz$Products$ProductInternal extends SF$Biz$Products$ProductBase {
	//更新时间
	//类型:datetime
	UpdatedTime: string;
	//
	//类型:datetime
	CreatedTime: string;
	//
	//类型:long
	ProductTypeId: number;
	//产品类型
	//类型:string
	ProductTypeName?: string;
}
// 
export interface SF$Biz$Products$ProductInternalQueryArgument {
	//产品ID
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//产品类型
	//类型:long
	ProductTypeId?: number;
	//更新时间
	//类型:SF.Entities.DateQueryRange
	UpdateTime?: SF$Entities$DateQueryRange;
	//价格区间
	//类型:QueryRange_decimal
	Price?: QueryRange_decimal;
	//产品名称
	//类型:string
	Name?: string;
	//状态
	//类型:SF.Entities.EntityLogicState
	State?: SF$Entities$EntityLogicState;
}
// 
export interface SF$Entities$DateQueryRange extends QueryRange_datetime {
	//
	//类型:datetime
	Begin?: string;
	//
	//类型:datetime
	End?: string;
}
// 
export interface QueryRange_decimal {
	//
	//类型:decimal
	Begin?: number;
	//
	//类型:decimal
	End?: number;
}
// 
export interface QueryResult_SF$Biz$Products$ProductInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Biz.Products.ProductInternal[]
	Items?: SF$Biz$Products$ProductInternal[];
}
// 
export interface SF$Biz$Products$ProductTypeEditable extends SF$Biz$Products$ProductType {
	//状态
	//类型:SF.Entities.EntityLogicState
	ObjectState: SF$Entities$EntityLogicState;
	//显示排位
	//类型:int
	Order?: number;
	//单位
	//类型:string
	Unit: string;
}
// 
export interface SF$Biz$Products$ProductType {
	//ID
	//类型:long
	Id: number;
	//名称
	//类型:string
	Name: string;
	//标题
	//类型:string
	Title: string;
	//图片
	//类型:string
	Image?: string;
	//图标
	//类型:string
	Icon?: string;
	//产品数量
	//类型:int
	ProductCount: number;
	//
	//类型:SF.Biz.Products.PropertyScope[]
	PropertyScopes?: SF$Biz$Products$PropertyScope[];
}
// 
export interface SF$Biz$Products$PropertyScope {
	//
	//类型:string
	Title?: string;
	//
	//类型:string
	Description?: string;
	//
	//类型:string
	Icon?: string;
	//
	//类型:string
	Image?: string;
	//
	//类型:SF.Biz.Products.Property[]
	Properties?: SF$Biz$Products$Property[];
}
// 
export interface SF$Biz$Products$Property {
	//
	//类型:string
	Title?: string;
	//
	//类型:string
	Description?: string;
	//
	//类型:string
	Icon?: string;
	//
	//类型:string
	Image?: string;
	//
	//类型:SF.Biz.Products.Property[]
	Children?: SF$Biz$Products$Property[];
}
// 
export interface SF$Biz$Products$ProductTypeInternal extends SF$Biz$Products$ProductTypeEditable {
	//更新时间
	//类型:datetime
	UpdatedTime: string;
	//
	//类型:datetime
	CreatedTime: string;
}
// 
export interface SF$Biz$Products$ProductTypeQueryArgument {
	//
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//对象状态
	//类型:SF.Entities.EntityLogicState
	ObjectState?: SF$Entities$EntityLogicState;
	//类型名称
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Biz$Products$ProductTypeInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Biz.Products.ProductTypeInternal[]
	Items?: SF$Biz$Products$ProductTypeInternal[];
}
// 
export interface SF$Biz$Products$CategoryInternal {
	//ID
	//类型:long
	Id: number;
	//名称
	//类型:string
	Name: string;
	//标题
	//类型:string
	Title: string;
	//销售人员
	//类型:long
	SellerId: number;
	//父目录
	//类型:long
	ParentId?: number;
	//父目录
	//类型:string
	ParentName?: string;
	//标签
	//类型:string
	Tag?: string;
	//描述
	//类型:string
	Description?: string;
	//图片
	//类型:string
	Image?: string;
	//图标
	//类型:string
	Icon?: string;
	//广告图
	//类型:string
	BannerImage?: string;
	//广告图链接
	//类型:string
	BannerUrl?: string;
	//移动站广告图
	//类型:string
	MobileBannerImage?: string;
	//移动站广告图链接
	//类型:string
	MobileBannerUrl?: string;
	//列表排位
	//类型:int
	Order?: number;
	//对象状态
	//类型:SF.Entities.EntityLogicState
	ObjectState: SF$Entities$EntityLogicState;
	//
	//类型:SF.Biz.Products.CategoryInternal[]
	Children?: SF$Biz$Products$CategoryInternal[];
	//
	//类型:long[]
	Items?: number[];
}
// 
export interface SF$Biz$Products$CategoryQueryArgument {
	//
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//卖家
	//类型:long
	SellerId?: number;
	//父目录
	//类型:long
	ParentId?: number;
	//对象状态
	//类型:SF.Entities.EntityLogicState
	ObjectState?: SF$Entities$EntityLogicState;
	//名称
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Biz$Products$CategoryInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Biz.Products.CategoryInternal[]
	Items?: SF$Biz$Products$CategoryInternal[];
}
// 
export interface SF$Biz$Products$ItemEditable {
	//
	//类型:long
	Id: number;
	//
	//类型:long
	SellerId: number;
	//
	//类型:long
	SourceItemId?: number;
	//
	//类型:long
	ProductId: number;
	//
	//类型:decimal
	Price?: number;
	//
	//类型:string
	Title?: string;
	//
	//类型:string
	Image?: string;
}
// 
export interface SF$Biz$Products$ItemInternal {
	//ID
	//类型:long
	Id: number;
	//
	//类型:long
	SourceItemId?: number;
	//产品
	//类型:long
	ProductId: number;
	//图片
	//类型:string
	Image?: string;
	//标题
	//类型:string
	Title?: string;
	//价格
	//类型:decimal
	Price?: number;
	//卡密
	//类型:bool
	IsVirtual?: boolean;
}
// 
export interface SF$Biz$Products$ItemQueryArgument {
	//
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//产品
	//类型:long
	ProductId?: number;
	//卖家
	//类型:long
	SellerId?: number;
	//产品标题
	//类型:string
	Title?: string;
	//产品目录
	//类型:long
	CategoryId?: number;
	//目录标签
	//类型:string
	CategoryTag?: string;
	//产品类型
	//类型:long
	TypeId?: number;
}
// 
export interface QueryResult_SF$Biz$Products$ItemInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Biz.Products.ItemInternal[]
	Items?: SF$Biz$Products$ItemInternal[];
}
// 
export interface SF$Biz$Products$IItem {
	//
	//类型:long
	ItemId: number;
	//
	//类型:long
	SellerId: number;
	//
	//类型:long
	ProductId: number;
	//
	//类型:string
	Title?: string;
	//
	//类型:string
	Image?: string;
	//
	//类型:decimal
	MarketPrice: number;
	//
	//类型:decimal
	Price: number;
	//
	//类型:string[]
	Tags?: string[];
	//
	//类型:datetime
	PublishedTime?: string;
	//
	//类型:int
	Visited: number;
	//
	//类型:long
	MainItemId: number;
	//
	//类型:bool
	IsVirtual?: boolean;
	//
	//类型:bool
	CouponDisabled?: boolean;
	//
	//类型:SF.Biz.Products.IProductImage[]
	Images?: SF$Biz$Products$IProductImage[];
	//
	//类型:SF.Biz.Products.IProductDescItem[]
	Descs?: SF$Biz$Products$IProductDescItem[];
	//
	//类型:bool
	OnSale?: boolean;
}
// 
export interface SF$Biz$Products$IProductImage {
	//
	//类型:string
	Image?: string;
	//
	//类型:string
	Title?: string;
}
// 
export interface SF$Biz$Products$IProductDescItem {
	//
	//类型:string
	Image?: string;
	//
	//类型:string
	Title?: string;
}
// 
export interface QueryResult_SF$Biz$Products$IItem {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Biz.Products.IItem[]
	Items?: SF$Biz$Products$IItem[];
}
// 
export interface SF$Biz$Products$ICategoryCached extends IEntityWithId_long {
	//
	//类型:string
	Tag?: string;
	//
	//类型:string
	Title?: string;
	//
	//类型:string
	Description?: string;
	//
	//类型:string
	Image?: string;
	//
	//类型:string
	Icon?: string;
	//
	//类型:int
	Order: number;
	//
	//类型:int
	ItemCount: number;
	//
	//类型:string
	BannerImage?: string;
	//
	//类型:string
	BannerUrl?: string;
	//
	//类型:string
	MobileBannerImage?: string;
	//
	//类型:string
	MobileBannerUrl?: string;
}
// 
export interface IEntityWithId_long {
	//
	//类型:long
	Id: number;
}
// 
export interface QueryResult_SF$Biz$Products$ICategoryCached {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Biz.Products.ICategoryCached[]
	Items?: SF$Biz$Products$ICategoryCached[];
}
// 
export interface SF$Users$Promotions$MemberInvitations$Models$MemberInvitationInternal extends SF$Data$Models$EventEntityBase {
	//被邀请人ID
	//类型:long
	Id: number;
	//被邀请人
	//类型:string
	InviteeName?: string;
	//邀请人ID
	//类型:long
	InvitorId: number;
	//邀请人
	//类型:string
	InvitorName?: string;
	//
	//类型:long[]
	Invitors?: number[];
}
// 
export interface SF$Users$Promotions$MemberInvitations$MemberInvitationQueryArgument {
	//Id
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//名称
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Users$Promotions$MemberInvitations$Models$MemberInvitationInternal {
	//
	//类型:SF.Entities.ISummary
	Summary?: SF$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Users.Promotions.MemberInvitations.Models.MemberInvitationInternal[]
	Items?: SF$Users$Promotions$MemberInvitations$Models$MemberInvitationInternal[];
}
// 
export interface SF$Core$NetworkService$Metadata$Library extends SF$Metadata$Models$Library {
	//
	//类型:SF.Core.NetworkService.Metadata.Service[]
	Services?: SF$Core$NetworkService$Metadata$Service[];
}
// 
export interface SF$Metadata$Models$Library {
	//
	//类型:SF.Metadata.Models.Type[]
	Types?: SF$Metadata$Models$Type[];
}
// 
export interface SF$Metadata$Models$Type extends SF$Metadata$Models$Entity {
	//
	//类型:string[]
	BaseTypes?: string[];
	//
	//类型:string
	ElementType?: string;
	//
	//类型:bool
	IsEnumType?: boolean;
	//
	//类型:bool
	IsArrayType?: boolean;
	//
	//类型:bool
	IsDictType?: boolean;
	//
	//类型:bool
	IsInterface?: boolean;
	//
	//类型:SF.Metadata.Models.Property[]
	Properties?: SF$Metadata$Models$Property[];
}
// 
export interface SF$Metadata$Models$Entity {
	//
	//类型:SF.Metadata.Models.Attribute[]
	Attributes?: SF$Metadata$Models$Attribute[];
	//
	//类型:string
	Name?: string;
	//
	//类型:string
	Title?: string;
	//
	//类型:string
	Description?: string;
	//
	//类型:string
	Group?: string;
	//
	//类型:string
	Prompt?: string;
	//
	//类型:string
	ShortName?: string;
	//
	//类型:string[]
	Categories?: string[];
}
// 
export interface SF$Metadata$Models$Attribute {
	//
	//类型:string
	Type?: string;
	//
	//类型:string
	Values?: string;
}
// 
export interface SF$Metadata$Models$Property extends SF$Metadata$Models$TypedEntity {
	//
	//类型:bool
	Optional?: boolean;
	//
	//类型:string
	DefaultValue?: string;
}
// 
export interface SF$Metadata$Models$TypedEntity extends SF$Metadata$Models$Entity {
	//
	//类型:string
	Type?: string;
}
// 
export interface SF$Core$NetworkService$Metadata$Service extends SF$Metadata$Models$Entity {
	//
	//类型:SF.Core.NetworkService.Metadata.Method[]
	Methods?: SF$Core$NetworkService$Metadata$Method[];
	//
	//类型:SF.Core.NetworkService.Metadata.GrantInfo
	GrantInfo?: SF$Core$NetworkService$Metadata$GrantInfo;
}
// 
export interface SF$Core$NetworkService$Metadata$Method extends SF$Metadata$Models$Method {
	//
	//类型:string
	HeavyParameter?: string;
	//
	//类型:SF.Core.NetworkService.Metadata.GrantInfo
	GrantInfo?: SF$Core$NetworkService$Metadata$GrantInfo;
}
// 
export interface SF$Metadata$Models$Method extends SF$Metadata$Models$TypedEntity {
	//
	//类型:SF.Metadata.Models.Parameter[]
	Parameters?: SF$Metadata$Models$Parameter[];
}
// 
export interface SF$Metadata$Models$Parameter extends SF$Metadata$Models$TypedEntity {
	//
	//类型:bool
	Optional?: boolean;
	//
	//类型:string
	DefaultValue?: string;
}
// 
export interface SF$Core$NetworkService$Metadata$GrantInfo {
	//
	//类型:bool
	UserRequired?: boolean;
	//
	//类型:string[]
	RolesRequired?: string[];
	//
	//类型:string[]
	PermissionsRequired?: string[];
}
//
//
export const ServiceFeatureControl={
//
//
Init(
	//
	//类型:string
	Id?: string,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'ServiceFeatureControl',
		'Init',
		{
			Id:Id
		},
		null,
		__opts
		);
},
}
//服务定义管理
//定义系统内置服务
export const ServiceDeclarationManager={
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Core$ServiceManagement$Models$ServiceDeclaration> {
	return _invoker(
		'ServiceDeclarationManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Core.ServiceManagement.Management.ServiceDeclarationQueryArgument
	Arg: SF$Core$ServiceManagement$Management$ServiceDeclarationQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Core$ServiceManagement$Models$ServiceDeclaration> {
	return _invoker(
		'ServiceDeclarationManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Core.ServiceManagement.Management.ServiceDeclarationQueryArgument
	Arg: SF$Core$ServiceManagement$Management$ServiceDeclarationQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'ServiceDeclarationManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
}
//服务实现管理
//系统内置服务实现
export const ServiceImplementManager={
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Core$ServiceManagement$Models$ServiceImplement> {
	return _invoker(
		'ServiceImplementManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Core.ServiceManagement.Management.ServiceImplementQueryArgument
	Arg: SF$Core$ServiceManagement$Management$ServiceImplementQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Core$ServiceManagement$Models$ServiceImplement> {
	return _invoker(
		'ServiceImplementManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Core.ServiceManagement.Management.ServiceImplementQueryArgument
	Arg: SF$Core$ServiceManagement$Management$ServiceImplementQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'ServiceImplementManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
}
//服务实例管理
//系统内置服务实例
export const ServiceInstanceManager={
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ServiceInstanceManager',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ServiceInstanceManager',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Core$ServiceManagement$Models$ServiceInstanceEditable> {
	return _invoker(
		'ServiceInstanceManager',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Core.ServiceManagement.Models.ServiceInstanceEditable
	Entity: SF$Core$ServiceManagement$Models$ServiceInstanceEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'ServiceInstanceManager',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Core.ServiceManagement.Models.ServiceInstanceEditable
	Entity: SF$Core$ServiceManagement$Models$ServiceInstanceEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ServiceInstanceManager',
		'Update',
		null,
		Entity,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Core$ServiceManagement$Models$ServiceInstanceInternal> {
	return _invoker(
		'ServiceInstanceManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Core.ServiceManagement.Management.ServiceInstanceQueryArgument
	Arg: SF$Core$ServiceManagement$Management$ServiceInstanceQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Core$ServiceManagement$Models$ServiceInstanceInternal> {
	return _invoker(
		'ServiceInstanceManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Core.ServiceManagement.Management.ServiceInstanceQueryArgument
	Arg: SF$Core$ServiceManagement$Management$ServiceInstanceQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'ServiceInstanceManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
}
//前端站点管理
//
export const SiteManager={
//
//
Remove(
	//
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteManager',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteManager',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$FrontEndContents$Site> {
	return _invoker(
		'SiteManager',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Management.FrontEndContents.Site
	Entity: SF$Management$FrontEndContents$Site,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_string> {
	return _invoker(
		'SiteManager',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Management.FrontEndContents.Site
	Entity: SF$Management$FrontEndContents$Site,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteManager',
		'Update',
		null,
		Entity,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$FrontEndContents$Site> {
	return _invoker(
		'SiteManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:QueryArgument_ObjectKey_string
	Arg: QueryArgument_ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Management$FrontEndContents$Site> {
	return _invoker(
		'SiteManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:QueryArgument_ObjectKey_string
	Arg: QueryArgument_ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'SiteManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
//
//
FindTemplateId(
	//
	//类型:string
	site: string,
	__opts?:ICallOptions
	) : PromiseLike<number> {
	return _invoker(
		'SiteManager',
		'FindTemplateId',
		{
			site:site
		},
		null,
		__opts
		);
},
}
//前端站点模板管理
//
export const SiteTemplateManager={
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteTemplateManager',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteTemplateManager',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$FrontEndContents$SiteTemplate> {
	return _invoker(
		'SiteTemplateManager',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Management.FrontEndContents.SiteTemplate
	Entity: SF$Management$FrontEndContents$SiteTemplate,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'SiteTemplateManager',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Management.FrontEndContents.SiteTemplate
	Entity: SF$Management$FrontEndContents$SiteTemplate,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteTemplateManager',
		'Update',
		null,
		Entity,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$FrontEndContents$SiteTemplate> {
	return _invoker(
		'SiteTemplateManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Management.FrontEndContents.SiteTemplateQueryArgument
	Arg: SF$Management$FrontEndContents$SiteTemplateQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Management$FrontEndContents$SiteTemplate> {
	return _invoker(
		'SiteTemplateManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Management.FrontEndContents.SiteTemplateQueryArgument
	Arg: SF$Management$FrontEndContents$SiteTemplateQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'SiteTemplateManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
//
//
LoadConfig(
	//
	//类型:long
	templateId: number,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'SiteTemplateManager',
		'LoadConfig',
		{
			templateId:templateId
		},
		null,
		__opts
		);
},
}
//前端内容管理
//
export const ContentManager={
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ContentManager',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ContentManager',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$FrontEndContents$Content> {
	return _invoker(
		'ContentManager',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Management.FrontEndContents.Content
	Entity: SF$Management$FrontEndContents$Content,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'ContentManager',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Management.FrontEndContents.Content
	Entity: SF$Management$FrontEndContents$Content,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ContentManager',
		'Update',
		null,
		Entity,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$FrontEndContents$Content> {
	return _invoker(
		'ContentManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Management.FrontEndContents.ContentQueryArgument
	Arg: SF$Management$FrontEndContents$ContentQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Management$FrontEndContents$Content> {
	return _invoker(
		'ContentManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Management.FrontEndContents.ContentQueryArgument
	Arg: SF$Management$FrontEndContents$ContentQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'ContentManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
//
//
LoadContent(
	//
	//类型:long
	contentId: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$FrontEndContents$IContent> {
	return _invoker(
		'ContentManager',
		'LoadContent',
		{
			contentId:contentId
		},
		null,
		__opts
		);
},
}
//文本消息记录
//
export const MsgRecordManager={
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$TextMessages$Management$MsgRecord> {
	return _invoker(
		'MsgRecordManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Common.TextMessages.Management.MsgRecordQueryArgument
	Arg: SF$Common$TextMessages$Management$MsgRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$TextMessages$Management$MsgRecord> {
	return _invoker(
		'MsgRecordManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Common.TextMessages.Management.MsgRecordQueryArgument
	Arg: SF$Common$TextMessages$Management$MsgRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'MsgRecordManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
}
//菜单管理
//
export const Menu={
//
//
GetMenu(
	//
	//类型:string
	Ident: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$MenuServices$MenuItem[]> {
	return _invoker(
		'Menu',
		'GetMenu',
		{
			Ident:Ident
		},
		null,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$MenuServices$Models$Menu> {
	return _invoker(
		'Menu',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Management.MenuServices.MenuQueryArgument
	Arg: SF$Management$MenuServices$MenuQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Management$MenuServices$Models$Menu> {
	return _invoker(
		'Menu',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Management.MenuServices.MenuQueryArgument
	Arg: SF$Management$MenuServices$MenuQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'Menu',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'Menu',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'Menu',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$MenuServices$Models$MenuEditable> {
	return _invoker(
		'Menu',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Management.MenuServices.Models.MenuEditable
	Entity: SF$Management$MenuServices$Models$MenuEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'Menu',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Management.MenuServices.Models.MenuEditable
	Entity: SF$Management$MenuServices$Models$MenuEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'Menu',
		'Update',
		null,
		Entity,
		__opts
		);
},
}
//身份认证管理
//
export const IdentityManagement={
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'IdentityManagement',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'IdentityManagement',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$Identities$Models$IdentityEditable> {
	return _invoker(
		'IdentityManagement',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Auth.Identities.Models.IdentityEditable
	Entity: SF$Auth$Identities$Models$IdentityEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'IdentityManagement',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Auth.Identities.Models.IdentityEditable
	Entity: SF$Auth$Identities$Models$IdentityEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'IdentityManagement',
		'Update',
		null,
		Entity,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$Identities$Models$IdentityInternal> {
	return _invoker(
		'IdentityManagement',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Auth.Identities.IdentityQueryArgument
	Arg: SF$Auth$Identities$IdentityQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Auth$Identities$Models$IdentityInternal> {
	return _invoker(
		'IdentityManagement',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Auth.Identities.IdentityQueryArgument
	Arg: SF$Auth$Identities$IdentityQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'IdentityManagement',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
}
//身份标识服务
//
export const Identity={
//获取当前身份标识ID
//
GetCurIdentityId(
	__opts?:ICallOptions
	) : PromiseLike<number> {
	return _invoker(
		'Identity',
		'GetCurIdentityId',
		null,
		null,
		__opts
		);
},
//获取当前身份标识
//
GetCurIdentity(
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$Identities$Models$Identity> {
	return _invoker(
		'Identity',
		'GetCurIdentity',
		null,
		null,
		__opts
		);
},
//登录
//
Signin(
	//
	//类型:SF.Auth.Identities.SigninArgument
	Arg: SF$Auth$Identities$SigninArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'Identity',
		'Signin',
		null,
		Arg,
		__opts
		);
},
//注销
//
Signout(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'Identity',
		'Signout',
		null,
		null,
		__opts
		);
},
//发送忘记密码验证消息
//
SendPasswordRecorveryCode(
	//
	//类型:SF.Auth.Identities.SendPasswordRecorveryCodeArgument
	Arg: SF$Auth$Identities$SendPasswordRecorveryCodeArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'Identity',
		'SendPasswordRecorveryCode',
		null,
		Arg,
		__opts
		);
},
//使用验证消息重置密码
//
ResetPasswordByRecoveryCode(
	//
	//类型:SF.Auth.Identities.ResetPasswordByRecorveryCodeArgument
	Arg: SF$Auth$Identities$ResetPasswordByRecorveryCodeArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'Identity',
		'ResetPasswordByRecoveryCode',
		null,
		Arg,
		__opts
		);
},
//设置密码
//
SetPassword(
	//
	//类型:SF.Auth.Identities.SetPasswordArgument
	Arg: SF$Auth$Identities$SetPasswordArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'Identity',
		'SetPassword',
		null,
		Arg,
		__opts
		);
},
//修改身份标识信息
//
UpdateIdentity(
	//
	//类型:SF.Auth.Identities.Models.Identity
	Identity: SF$Auth$Identities$Models$Identity,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'Identity',
		'UpdateIdentity',
		null,
		Identity,
		__opts
		);
},
//从访问令牌提取身份ID
//
ParseAccessToken(
	//
	//类型:string
	AccessToken: string,
	__opts?:ICallOptions
	) : PromiseLike<number> {
	return _invoker(
		'Identity',
		'ParseAccessToken',
		{
			AccessToken:AccessToken
		},
		null,
		__opts
		);
},
//发送身份标识创建验证信息
//
SendCreateIdentityVerifyCode(
	//
	//类型:SF.Auth.Identities.SendCreateIdentityVerifyCodeArgument
	Arg: SF$Auth$Identities$SendCreateIdentityVerifyCodeArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'Identity',
		'SendCreateIdentityVerifyCode',
		null,
		Arg,
		__opts
		);
},
//创建身份标识
//
CreateIdentity(
	//
	//类型:SF.Auth.Identities.CreateIdentityArgument
	Arg: SF$Auth$Identities$CreateIdentityArgument,
	//
	//类型:bool
	VerifyCode: boolean,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'Identity',
		'CreateIdentity',
		{
			VerifyCode:VerifyCode
		},
		Arg,
		__opts
		);
},
//根据身份标识ID获取身份信息
//
GetIdentity(
	//
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$Identities$Models$Identity> {
	return _invoker(
		'Identity',
		'GetIdentity',
		{
			Id:Id
		},
		null,
		__opts
		);
},
}
//
//
export const Member={
//
//
Signup(
	//
	//类型:SF.Auth.Identities.CreateIdentityArgument
	Arg: SF$Auth$Identities$CreateIdentityArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'Member',
		'Signup',
		null,
		Arg,
		__opts
		);
},
//
//
GetCurrentUser(
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$Users$Models$UserDesc> {
	return _invoker(
		'Member',
		'GetCurrentUser',
		null,
		null,
		__opts
		);
},
}
//会员
//
export const MemberManagement={
//
//
CreateUser(
	//
	//类型:SF.Auth.Identities.CreateIdentityArgument
	Arg: SF$Auth$Identities$CreateIdentityArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'MemberManagement',
		'CreateUser',
		null,
		Arg,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Users$Members$Models$MemberInternal> {
	return _invoker(
		'MemberManagement',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Users.Members.MemberQueryArgument
	Arg: SF$Users$Members$MemberQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Users$Members$Models$MemberInternal> {
	return _invoker(
		'MemberManagement',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Users.Members.MemberQueryArgument
	Arg: SF$Users$Members$MemberQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'MemberManagement',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MemberManagement',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MemberManagement',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Users$Members$Models$MemberEditable> {
	return _invoker(
		'MemberManagement',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Users.Members.Models.MemberEditable
	Entity: SF$Users$Members$Models$MemberEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'MemberManagement',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Users.Members.Models.MemberEditable
	Entity: SF$Users$Members$Models$MemberEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MemberManagement',
		'Update',
		null,
		Entity,
		__opts
		);
},
}
//系统管理员
//
export const SysAdminManagement={
//
//
CreateUser(
	//
	//类型:SF.Auth.Identities.CreateIdentityArgument
	Arg: SF$Auth$Identities$CreateIdentityArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'SysAdminManagement',
		'CreateUser',
		null,
		Arg,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$SysAdmins$Models$SysAdminInternal> {
	return _invoker(
		'SysAdminManagement',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Management.SysAdmins.SysAdminQueryArgument
	Arg: SF$Management$SysAdmins$SysAdminQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Management$SysAdmins$Models$SysAdminInternal> {
	return _invoker(
		'SysAdminManagement',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Management.SysAdmins.SysAdminQueryArgument
	Arg: SF$Management$SysAdmins$SysAdminQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'SysAdminManagement',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SysAdminManagement',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SysAdminManagement',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$SysAdmins$Models$SysAdminEditable> {
	return _invoker(
		'SysAdminManagement',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Management.SysAdmins.Models.SysAdminEditable
	Entity: SF$Management$SysAdmins$Models$SysAdminEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'SysAdminManagement',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Management.SysAdmins.Models.SysAdminEditable
	Entity: SF$Management$SysAdmins$Models$SysAdminEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SysAdminManagement',
		'Update',
		null,
		Entity,
		__opts
		);
},
}
//
//
export const SysAdmin={
//
//
Signup(
	//
	//类型:SF.Auth.Identities.CreateIdentityArgument
	Arg: SF$Auth$Identities$CreateIdentityArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'SysAdmin',
		'Signup',
		null,
		Arg,
		__opts
		);
},
//
//
GetCurrentUser(
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$Users$Models$UserDesc> {
	return _invoker(
		'SysAdmin',
		'GetCurrentUser',
		null,
		null,
		__opts
		);
},
}
//业务管理员
//
export const BizAdminManagement={
//
//
CreateUser(
	//
	//类型:SF.Auth.Identities.CreateIdentityArgument
	Arg: SF$Auth$Identities$CreateIdentityArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'BizAdminManagement',
		'CreateUser',
		null,
		Arg,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$BizAdmins$Models$BizAdminInternal> {
	return _invoker(
		'BizAdminManagement',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Management.BizAdmins.BizAdminQueryArgument
	Arg: SF$Management$BizAdmins$BizAdminQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Management$BizAdmins$Models$BizAdminInternal> {
	return _invoker(
		'BizAdminManagement',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Management.BizAdmins.BizAdminQueryArgument
	Arg: SF$Management$BizAdmins$BizAdminQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'BizAdminManagement',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'BizAdminManagement',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'BizAdminManagement',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$BizAdmins$Models$BizAdminEditable> {
	return _invoker(
		'BizAdminManagement',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Management.BizAdmins.Models.BizAdminEditable
	Entity: SF$Management$BizAdmins$Models$BizAdminEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'BizAdminManagement',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Management.BizAdmins.Models.BizAdminEditable
	Entity: SF$Management$BizAdmins$Models$BizAdminEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'BizAdminManagement',
		'Update',
		null,
		Entity,
		__opts
		);
},
}
//
//
export const BizAdmin={
//
//
Signup(
	//
	//类型:SF.Auth.Identities.CreateIdentityArgument
	Arg: SF$Auth$Identities$CreateIdentityArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'BizAdmin',
		'Signup',
		null,
		Arg,
		__opts
		);
},
//
//
GetCurrentUser(
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$Users$Models$UserDesc> {
	return _invoker(
		'BizAdmin',
		'GetCurrentUser',
		null,
		null,
		__opts
		);
},
}
//分类管理
//
export const DocumentCategoryManager={
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Management$CategoryInternal> {
	return _invoker(
		'DocumentCategoryManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Common.Documents.Management.DocumentCategoryQueryArgument
	Arg: SF$Common$Documents$Management$DocumentCategoryQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Documents$Management$CategoryInternal> {
	return _invoker(
		'DocumentCategoryManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Common.Documents.Management.DocumentCategoryQueryArgument
	Arg: SF$Common$Documents$Management$DocumentCategoryQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DocumentCategoryManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentCategoryManager',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentCategoryManager',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Management$CategoryInternal> {
	return _invoker(
		'DocumentCategoryManager',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Common.Documents.Management.CategoryInternal
	Entity: SF$Common$Documents$Management$CategoryInternal,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DocumentCategoryManager',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Common.Documents.Management.CategoryInternal
	Entity: SF$Common$Documents$Management$CategoryInternal,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentCategoryManager',
		'Update',
		null,
		Entity,
		__opts
		);
},
}
//文档管理
//
export const DocumentManager={
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Management$DocumentInternal> {
	return _invoker(
		'DocumentManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Common.Documents.Management.DocumentQueryArguments
	Arg: SF$Common$Documents$Management$DocumentQueryArguments,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Documents$Management$DocumentInternal> {
	return _invoker(
		'DocumentManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Common.Documents.Management.DocumentQueryArguments
	Arg: SF$Common$Documents$Management$DocumentQueryArguments,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DocumentManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentManager',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentManager',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Management$DocumentEditable> {
	return _invoker(
		'DocumentManager',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Common.Documents.Management.DocumentEditable
	Entity: SF$Common$Documents$Management$DocumentEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DocumentManager',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Common.Documents.Management.DocumentEditable
	Entity: SF$Common$Documents$Management$DocumentEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentManager',
		'Update',
		null,
		Entity,
		__opts
		);
},
}
//文档服务
//
export const Document={
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Document> {
	return _invoker(
		'Document',
		'Get',
		null,
		Id,
		__opts
		);
},
//通过快速访问键值获取对象
//
GetByKey(
	//
	//类型:string
	Key: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Document> {
	return _invoker(
		'Document',
		'GetByKey',
		{
			Key:Key
		},
		null,
		__opts
		);
},
//通过ID获取容器对象
//
LoadContainer(
	//
	//类型:long
	Key: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Category> {
	return _invoker(
		'Document',
		'LoadContainer',
		{
			Key:Key
		},
		null,
		__opts
		);
},
//通过关键字搜索对象
//
Search(
	//
	//类型:string
	Key: string,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Documents$Document> {
	return _invoker(
		'Document',
		'Search',
		{
			Key:Key
		},
		null,
		__opts
		);
},
//获取容器中的对象
//
ListItems(
	//
	//类型:long
	Container: number,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Documents$Document> {
	return _invoker(
		'Document',
		'ListItems',
		{
			Container:Container
		},
		null,
		__opts
		);
},
//
//
ListChildContainers(
	//
	//类型:long
	Key: number,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Documents$Category> {
	return _invoker(
		'Document',
		'ListChildContainers',
		{
			Key:Key
		},
		null,
		__opts
		);
},
}
//产品管理
//
export const ProductManager={
//
//
SetLogicState(
	//
	//类型:long
	Id: number,
	//
	//类型:SF.Entities.EntityLogicState
	State: SF$Entities$EntityLogicState,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductManager',
		'SetLogicState',
		{
			Id:Id,
			State:State
		},
		null,
		__opts
		);
},
//
//
GetSpec(
	//
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$ProductSpec> {
	return _invoker(
		'ProductManager',
		'GetSpec',
		{
			Id:Id
		},
		null,
		__opts
		);
},
//
//
ListSpec(
	//
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$ProductSpec[]> {
	return _invoker(
		'ProductManager',
		'ListSpec',
		{
			Id:Id
		},
		null,
		__opts
		);
},
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductManager',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductManager',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$ProductEditable> {
	return _invoker(
		'ProductManager',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Biz.Products.ProductEditable
	Entity: SF$Biz$Products$ProductEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'ProductManager',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Biz.Products.ProductEditable
	Entity: SF$Biz$Products$ProductEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductManager',
		'Update',
		null,
		Entity,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$ProductInternal> {
	return _invoker(
		'ProductManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Biz.Products.ProductInternalQueryArgument
	Arg: SF$Biz$Products$ProductInternalQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Biz$Products$ProductInternal> {
	return _invoker(
		'ProductManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Biz.Products.ProductInternalQueryArgument
	Arg: SF$Biz$Products$ProductInternalQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'ProductManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
}
//产品类型管理
//
export const ProductTypeManager={
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductTypeManager',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductTypeManager',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$ProductTypeEditable> {
	return _invoker(
		'ProductTypeManager',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Biz.Products.ProductTypeEditable
	Entity: SF$Biz$Products$ProductTypeEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'ProductTypeManager',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Biz.Products.ProductTypeEditable
	Entity: SF$Biz$Products$ProductTypeEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductTypeManager',
		'Update',
		null,
		Entity,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$ProductTypeInternal> {
	return _invoker(
		'ProductTypeManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Biz.Products.ProductTypeQueryArgument
	Arg: SF$Biz$Products$ProductTypeQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Biz$Products$ProductTypeInternal> {
	return _invoker(
		'ProductTypeManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Biz.Products.ProductTypeQueryArgument
	Arg: SF$Biz$Products$ProductTypeQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'ProductTypeManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
}
//产品目录管理
//
export const ProductCategoryManager={
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductCategoryManager',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductCategoryManager',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$CategoryInternal> {
	return _invoker(
		'ProductCategoryManager',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Biz.Products.CategoryInternal
	Entity: SF$Biz$Products$CategoryInternal,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'ProductCategoryManager',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Biz.Products.CategoryInternal
	Entity: SF$Biz$Products$CategoryInternal,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductCategoryManager',
		'Update',
		null,
		Entity,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$CategoryInternal> {
	return _invoker(
		'ProductCategoryManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Biz.Products.CategoryQueryArgument
	Arg: SF$Biz$Products$CategoryQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Biz$Products$CategoryInternal> {
	return _invoker(
		'ProductCategoryManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Biz.Products.CategoryQueryArgument
	Arg: SF$Biz$Products$CategoryQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'ProductCategoryManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
}
//商品管理
//
export const ProductItemManager={
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductItemManager',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductItemManager',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$ItemEditable> {
	return _invoker(
		'ProductItemManager',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Biz.Products.ItemEditable
	Entity: SF$Biz$Products$ItemEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'ProductItemManager',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Biz.Products.ItemEditable
	Entity: SF$Biz$Products$ItemEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ProductItemManager',
		'Update',
		null,
		Entity,
		__opts
		);
},
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$ItemInternal> {
	return _invoker(
		'ProductItemManager',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Biz.Products.ItemQueryArgument
	Arg: SF$Biz$Products$ItemQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Biz$Products$ItemInternal> {
	return _invoker(
		'ProductItemManager',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Biz.Products.ItemQueryArgument
	Arg: SF$Biz$Products$ItemQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'ProductItemManager',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
}
//
//
export const Item={
//
//
GetItemDetail(
	//
	//类型:long
	ItemId: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$IItem> {
	return _invoker(
		'Item',
		'GetItemDetail',
		{
			ItemId:ItemId
		},
		null,
		__opts
		);
},
//
//
GetProductDetail(
	//
	//类型:long
	ProductId: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$IItem> {
	return _invoker(
		'Item',
		'GetProductDetail',
		{
			ProductId:ProductId
		},
		null,
		__opts
		);
},
//
//
GetProducts(
	//
	//类型:long[]
	ProductIds: number[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$IItem[]> {
	return _invoker(
		'Item',
		'GetProducts',
		null,
		ProductIds,
		__opts
		);
},
//
//
GetItems(
	//
	//类型:long[]
	ItemIds: number[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$IItem[]> {
	return _invoker(
		'Item',
		'GetItems',
		null,
		ItemIds,
		__opts
		);
},
//
//
ListTaggedItems(
	//
	//类型:string
	Tag: string,
	//
	//类型:bool
	WithChildCategoryItems: boolean,
	//
	//类型:string
	Filter: string,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Biz$Products$IItem> {
	return _invoker(
		'Item',
		'ListTaggedItems',
		{
			Tag:Tag,
			WithChildCategoryItems:WithChildCategoryItems,
			Filter:Filter
		},
		null,
		__opts
		);
},
//
//
ListCategoryItems(
	//
	//类型:long
	CategoryId: number,
	//
	//类型:bool
	WithChildCategoryItems: boolean,
	//
	//类型:string
	Filter: string,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Biz$Products$IItem> {
	return _invoker(
		'Item',
		'ListCategoryItems',
		{
			CategoryId:CategoryId,
			WithChildCategoryItems:WithChildCategoryItems,
			Filter:Filter
		},
		null,
		__opts
		);
},
//
//
GetTaggedCategories(
	//
	//类型:string
	Tag: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$ICategoryCached[]> {
	return _invoker(
		'Item',
		'GetTaggedCategories',
		{
			Tag:Tag
		},
		null,
		__opts
		);
},
//
//
GetCategories(
	//
	//类型:long[]
	CategoryIds: number[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Biz$Products$ICategoryCached[]> {
	return _invoker(
		'Item',
		'GetCategories',
		null,
		CategoryIds,
		__opts
		);
},
//
//
ListCategories(
	//
	//类型:long
	ParentCategoryId: number,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Biz$Products$ICategoryCached> {
	return _invoker(
		'Item',
		'ListCategories',
		{
			ParentCategoryId:ParentCategoryId
		},
		null,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Biz.Products.ItemQueryArgument
	Arg: SF$Biz$Products$ItemQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Biz$Products$IItem> {
	return _invoker(
		'Item',
		'Query',
		null,
		Arg,
		__opts
		);
},
}
//会员邀请
//
export const MemberInvitationManagement={
//通过主键获取对象
//
Get(
	//
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Users$Promotions$MemberInvitations$Models$MemberInvitationInternal> {
	return _invoker(
		'MemberInvitationManagement',
		'Get',
		null,
		Id,
		__opts
		);
},
//
//
Query(
	//
	//类型:SF.Users.Promotions.MemberInvitations.MemberInvitationQueryArgument
	Arg: SF$Users$Promotions$MemberInvitations$MemberInvitationQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Users$Promotions$MemberInvitations$Models$MemberInvitationInternal> {
	return _invoker(
		'MemberInvitationManagement',
		'Query',
		null,
		Arg,
		__opts
		);
},
//
//
QueryIdents(
	//
	//类型:SF.Users.Promotions.MemberInvitations.MemberInvitationQueryArgument
	Arg: SF$Users$Promotions$MemberInvitations$MemberInvitationQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'MemberInvitationManagement',
		'QueryIdents',
		null,
		Arg,
		__opts
		);
},
//
//
Remove(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MemberInvitationManagement',
		'Remove',
		null,
		Key,
		__opts
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MemberInvitationManagement',
		'RemoveAll',
		null,
		null,
		__opts
		);
},
//
//
LoadForEdit(
	//
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Users$Promotions$MemberInvitations$Models$MemberInvitationInternal> {
	return _invoker(
		'MemberInvitationManagement',
		'LoadForEdit',
		null,
		Key,
		__opts
		);
},
//
//
Create(
	//
	//类型:SF.Users.Promotions.MemberInvitations.Models.MemberInvitationInternal
	Entity: SF$Users$Promotions$MemberInvitations$Models$MemberInvitationInternal,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'MemberInvitationManagement',
		'Create',
		null,
		Entity,
		__opts
		);
},
//
//
Update(
	//
	//类型:SF.Users.Promotions.MemberInvitations.Models.MemberInvitationInternal
	Entity: SF$Users$Promotions$MemberInvitations$Models$MemberInvitationInternal,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MemberInvitationManagement',
		'Update',
		null,
		Entity,
		__opts
		);
},
}
//媒体附件支持
//
export const Media={
//
//
Upload(
	//
	//类型:bool
	returnJson?: boolean,
	__opts?:ICallOptions
	) : PromiseLike<any> {
	return _invoker(
		'Media',
		'Upload',
		{
			returnJson:returnJson
		},
		null,
		__opts
		);
},
//
//
Clip(
	//
	//类型:string
	src: string,
	//
	//类型:double
	x: number,
	//
	//类型:double
	y: number,
	//
	//类型:double
	w: number,
	//
	//类型:double
	h: number,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'Media',
		'Clip',
		{
			src:src,
			x:x,
			y:y,
			w:w,
			h:h
		},
		null,
		__opts
		);
},
//
//
Get(
	//
	//类型:string
	id: string,
	//
	//类型:string
	format?: string,
	__opts?:ICallOptions
	) : PromiseLike<any> {
	return _invoker(
		'Media',
		'Get',
		{
			id:id,
			format:format
		},
		null,
		__opts
		);
},
}
//
//
export const ServiceMetadata={
//
//
Json(
	__opts?:ICallOptions
	) : PromiseLike<SF$Core$NetworkService$Metadata$Library> {
	return _invoker(
		'ServiceMetadata',
		'Json',
		null,
		null,
		__opts
		);
},
//
//
Typescript(
	//
	//类型:bool
	all?: boolean,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'ServiceMetadata',
		'Typescript',
		{
			all:all
		},
		null,
		__opts
		);
},
}
