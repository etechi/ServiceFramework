
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
	(type: string,method: string,post:string, args : { [index: string]: any}, opts?: ICallOptions) :any
}

var _invoker:IApiInvoker=null;
export function setApiInvoker(invoker:IApiInvoker){
	_invoker=invoker;
}


// 
export interface SF$Sys$Hosting$FilePathDefination {
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
export interface SF$Sys$Hosting$FilePathResolverSettingType {
	//DefaultFilePathDefinationSource
	//类型:long
	DefaultFilePathDefinationSource?: number;
	//Setting
	//类型:SF.Sys.Hosting.FilePathDefination
	Setting: SF$Sys$Hosting$FilePathDefination;
}
// 
export interface SF$Sys$Caching$LocalFileCacheSetting {
	//缓存根目录
	//类型:string
	RootPath: string;
}
// 
export interface SF$Sys$Caching$LocalFileCacheSettingType {
	//Setting
	//类型:SF.Sys.Caching.LocalFileCacheSetting
	Setting: SF$Sys$Caching$LocalFileCacheSetting;
}
// 
export interface SF$Sys$Security$DataProtectorSettingType {
	//GlobalPassword
	//类型:string
	GlobalPassword: string;
}
// 
export interface SF$Sys$Security$PasswordHasherSettingType {
	//
	//类型:string
	GlobalPassword: string;
}
// 站点设置
export interface SF$Sys$Settings$AppSiteSetting {
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
export interface SF$Sys$Settings$SettingService$SF$Sys$Settings$AppSiteSetting$SettingType {
	//Value
	//类型:SF.Sys.Settings.AppSiteSetting
	Value: SF$Sys$Settings$AppSiteSetting;
}
// 客服设置
export interface SF$Sys$Settings$CustomServiceSetting {
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
export interface SF$Sys$Settings$SettingService$SF$Sys$Settings$CustomServiceSetting$SettingType {
	//Value
	//类型:SF.Sys.Settings.CustomServiceSetting
	Value: SF$Sys$Settings$CustomServiceSetting;
}
// 调试设置
export interface SF$Sys$Settings$DebugSetting {
	//调试模式
	//类型:bool
	DebugMode?: boolean;
	//调试用户ID
	//类型:int
	DebugUserId: number;
}
// 
export interface SF$Sys$Settings$SettingService$SF$Sys$Settings$DebugSetting$SettingType {
	//Value
	//类型:SF.Sys.Settings.DebugSetting
	Value: SF$Sys$Settings$DebugSetting;
}
// HTTP设置
export interface SF$Sys$Settings$HttpSetting {
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
export interface SF$Sys$Settings$SettingService$SF$Sys$Settings$HttpSetting$SettingType {
	//Value
	//类型:SF.Sys.Settings.HttpSetting
	Value: SF$Sys$Settings$HttpSetting;
}
// 系统设置
export interface SF$Sys$Settings$SystemSetting {
	//系统名称
	//类型:string
	SystemName?: string;
}
// 
export interface SF$Sys$Settings$SettingService$SF$Sys$Settings$SystemSetting$SettingType {
	//Value
	//类型:SF.Sys.Settings.SystemSetting
	Value: SF$Sys$Settings$SystemSetting;
}
// 
export interface SF$Common$Media$MediaManagerSettingType {
	//MetaCache
	//类型:long
	MetaCache?: number;
}
// 媒体WebApi配置
export interface SF$Common$Media$MediaServiceSetting {
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
export interface SF$Common$Media$MediaServiceSettingType {
	//Manager
	//类型:long
	Manager?: number;
	//Setting
	//类型:SF.Common.Media.MediaServiceSetting
	Setting: SF$Common$Media$MediaServiceSetting;
	//MimeResolver
	//类型:long
	MimeResolver?: number;
	//FileCollection
	//类型:long
	FileCollection?: number;
	//InvokeContext
	//类型:long
	InvokeContext?: number;
	//ImageProvider
	//类型:long
	ImageProvider?: number;
	//FileCache
	//类型:long
	FileCache?: number;
}
// 
export interface SF$Common$Media$Storages$FileSystemMediaStorageSettingType {
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
export interface SF$Common$Media$Storages$StaticFileMediaStorageSettingType {
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
export interface Lazy_IDataSet_SF$Sys$MenuServices$Entity$DataModels$MenuItem {
	//
	//类型:bool
	IsValueCreated?: boolean;
}
// 
export interface SF$Sys$MenuServices$Entity$EntityMenuService$SF$Sys$MenuServices$Entity$DataModels$Menu_SF$Sys$MenuServices$Entity$DataModels$MenuItem$SettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
	//MenuItemSet
	//类型:Lazy_IDataSet_SF.Sys.MenuServices.Entity.DataModels.MenuItem
	MenuItemSet: Lazy_IDataSet_SF$Sys$MenuServices$Entity$DataModels$MenuItem;
}
// 
export interface KDL$AppSetting {
	//
	//类型:long
	PCHelpCenterDefaultDocId: number;
}
// 
export interface SF$Sys$Settings$SettingService$KDL$AppSetting$SettingType {
	//Value
	//类型:KDL.AppSetting
	Value: KDL$AppSetting;
}
// 
export interface KDL$Services$Treatments$Patients$PatientManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Patients$PatientDiseaseManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Patients$PharmaconHistoryManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Medications$MedicationPlanManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Medications$MedicationRecordManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Medications$MedicineBoxCellSettingManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Medications$MedicineBoxReloadPlanManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Diagnostics$DiagnosticRecordManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Diagnostics$DiagnosticRecordItemManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Reminds$RemindPlanManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Reminds$RemindActionManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Treatments$Reminds$RemindRecordManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Devices$DeviceManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Devices$DeviceModelManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Devices$DeviceProductManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Devices$DeviceBatchManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Doctors$DoctorManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Pharmacons$PharmaconManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Pharmacons$PharmaconSpecManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Pharmacons$PharmaconCategoryManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Pharmacons$PharmaconUnitManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface KDL$Services$Diseases$DiseaseManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Auth$IdentityServices$Managers$ClaimTypeManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Auth$IdentityServices$Managers$ClientConfigManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Auth$IdentityServices$Managers$ClientManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Auth$IdentityServices$Managers$OperationManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Auth$IdentityServices$Managers$ResourceManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Auth$IdentityServices$Managers$RoleManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Auth$IdentityServices$Managers$UserManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
	//PasswordHasher
	//类型:long
	PasswordHasher?: number;
}
// 
export interface SF$Auth$IdentityServices$Managers$ScopeManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Auth$IdentityServices$UserServiceSetting {
	//
	//类型:bool
	VerifyCodeDisabled?: boolean;
	//
	//类型:ILocalCache_SF.Auth.IdentityServices.VerifyCode
	VerifyCodeCache?: ILocalCache_SF$Auth$IdentityServices$VerifyCode;
	//IdentStorage
	//类型:long
	IdentStorage?: number;
	//CredentialStorage
	//类型:long
	CredentialStorage?: number;
	//ClientService
	//类型:long
	ClientService?: number;
	//PasswordHasher
	//类型:long
	PasswordHasher?: number;
	//ServiceInstanceDescriptor
	//类型:long
	ServiceInstanceDescriptor?: number;
	//TimeService
	//类型:long
	TimeService?: number;
	//
	//类型:ILocalCache_SF.Auth.IdentityServices.Internals.UserData
	IdentityDataCache?: ILocalCache_SF$Auth$IdentityServices$Internals$UserData;
	//
	//类型:string
	DefaultIdentityCredentialProvider?: string;
	//AccessTokenGenerator
	//类型:long
	AccessTokenGenerator?: number;
}
// 
export interface ILocalCache_SF$Auth$IdentityServices$VerifyCode {
}
// 
export interface ILocalCache_SF$Auth$IdentityServices$Internals$UserData {
}
// 
export interface SF$Auth$IdentityServices$UserServiceSettingType {
	//Setting
	//类型:SF.Auth.IdentityServices.UserServiceSetting
	Setting: SF$Auth$IdentityServices$UserServiceSetting;
}
// 
export interface SF$Auth$IdentityServices$UserCredentialProviders$PhoneNumberUserCredentialProviderSettingType {
	//PhoneNumberValidator
	//类型:long
	PhoneNumberValidator?: number;
	//TextMessageService
	//类型:long
	TextMessageService?: number;
}
// 
export interface SF$Auth$IdentityServices$UserCredentialProviders$LocalUserCredentialProviderSettingType {
}
// 
export interface SF$Auth$IdentityServices$Managers$UserCredentialStorageSettingType {
	//TimeService
	//类型:long
	TimeService?: number;
	//ServiceInstanceDescriptor
	//类型:long
	ServiceInstanceDescriptor?: number;
}
// 
export interface SF$Common$TextMessages$Management$EntityMsgRecordManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Common$TextMessages$Management$EntityMsgActionRecordManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Common$TextMessages$Management$EntityMsgPolicyManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Common$TextMessages$EMailProviders$SystemEMailSetting {
	//禁止发送
	//类型:bool
	Disabled?: boolean;
	//用户
	//类型:string
	User: string;
	//密码
	//类型:string
	Password: string;
	//SMTP服务器地址
	//类型:string
	SMTPServerAddress: string;
	//SMTP服务器端口
	//类型:int
	SMTPServerPort: number;
}
// 
export interface SF$Common$TextMessages$EMailProviders$SystemEMailProviderSettingType {
	//Setting
	//类型:SF.Common.TextMessages.EMailProviders.SystemEMailSetting
	Setting: SF$Common$TextMessages$EMailProviders$SystemEMailSetting;
	//TimeService
	//类型:long
	TimeService?: number;
	//UserPropertyResolver
	//类型:long
	UserPropertyResolver?: number;
}
// 
export interface SF$Common$Members$MemberManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Common$Admins$AdminManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Common$FrontEndContents$SiteManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Common$FrontEndContents$SiteTemplateManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Common$FrontEndContents$ContentManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Common$FrontEndContents$Friendly$FriendlyContentSetting {
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
export interface SF$Sys$Settings$SettingService$SF$Common$FrontEndContents$Friendly$FriendlyContentSetting$SettingType {
	//Value
	//类型:SF.Common.FrontEndContents.Friendly.FriendlyContentSetting
	Value: SF$Common$FrontEndContents$Friendly$FriendlyContentSetting;
}
// 
export interface SF$Common$Documents$Management$DocumentCategoryManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
}
// 
export interface SF$Common$Documents$Management$DocumentManagerSettingType {
	//ServiceContext
	//类型:long
	ServiceContext?: number;
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
	//Documents
	//类型:Lazy_IDataSet_SF.Common.Documents.DataModels.Document
	Documents: Lazy_IDataSet_SF$Common$Documents$DataModels$Document;
	//Categories
	//类型:Lazy_IDataSet_SF.Common.Documents.DataModels.DocumentCategory
	Categories: Lazy_IDataSet_SF$Common$Documents$DataModels$DocumentCategory;
	//ServiceInstanceDescriptor
	//类型:long
	ServiceInstanceDescriptor?: number;
}
// 
export interface ObjectKey_string {
	//标识
	//类型:string
	Id?: string;
}
// 
export interface SF$Sys$Services$Management$Models$ServiceDeclaration {
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
export interface SF$Sys$Services$Management$ServiceDeclarationQueryArgument extends QueryArgument_ObjectKey_string {
	//服务定义名称
	//类型:string
	Name?: string;
	//服务定义分类
	//类型:string
	Group?: string;
}
// 
export interface QueryArgument_ObjectKey_string {
	//
	//类型:ObjectKey_string
	Id?: ObjectKey_string;
	//
	//类型:SF.Sys.Entities.Paging
	Paging?: SF$Sys$Entities$Paging;
}
// 
export interface SF$Sys$Entities$Paging {
	//起始记录
	//类型:int
	Offset: number;
	//返回记录条数
	//类型:int
	Count: number;
	//排序方式
	//类型:string
	SortMethod?: string;
	//排序类型
	//类型:SF.Sys.Entities.SortOrder
	SortOrder: SF$Sys$Entities$SortOrder;
	//返回总数
	//类型:bool
	TotalRequired?: boolean;
	//返回摘要
	//类型:bool
	SummaryRequired?: boolean;
}
// 
export type SF$Sys$Entities$SortOrder = 'Default'|'Asc'|'Desc'|'Random';
export const SF$Sys$Entities$SortOrderNames={
	"Default":"默认",
	"Asc":"升序",
	"Desc":"降序",
	"Random":"随机排序",
}
// 
export interface QueryResult_SF$Sys$Services$Management$Models$ServiceDeclaration {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Sys.Services.Management.Models.ServiceDeclaration[]
	Items?: SF$Sys$Services$Management$Models$ServiceDeclaration[];
}
// 
export interface SF$Sys$Entities$ISummary {
}
// 
export interface QueryResult_ObjectKey_string {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:ObjectKey_string[]
	Items?: ObjectKey_string[];
}
// 
export interface SF$Sys$Services$Management$Models$ServiceImplement {
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
export interface SF$Sys$Services$Management$ServiceImplementQueryArgument extends QueryArgument_ObjectKey_string {
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
export interface QueryResult_SF$Sys$Services$Management$Models$ServiceImplement {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Sys.Services.Management.Models.ServiceImplement[]
	Items?: SF$Sys$Services$Management$Models$ServiceImplement[];
}
// 
export interface ObjectKey_long {
	//标识
	//类型:long
	Id: number;
}
// 
export interface SF$Sys$Services$Management$Models$ServiceInstanceEditable extends SF$Sys$Services$Management$Models$ServiceInstanceInternal {
	//服务设置
	//类型:string
	Setting?: string;
	//
	//类型:string
	SettingType?: string;
}
// 
export interface SF$Sys$Services$Management$Models$ServiceInstanceInternal extends SF$Sys$Services$Management$Models$ServiceInstance {
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
	//类型:SF.Sys.Services.Management.Models.ServiceInstanceInternal[]
	Children?: SF$Sys$Services$Management$Models$ServiceInstanceInternal[];
}
// 
export interface SF$Sys$Services$Management$Models$ServiceInstance extends UIObjectEntityBase_long {
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
	//类型:SF.Sys.Entities.EntityLogicState
	LogicState: SF$Sys$Entities$EntityLogicState;
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
export type SF$Sys$Entities$EntityLogicState = 'Enabled'|'Disabled'|'Deleted';
export const SF$Sys$Entities$EntityLogicStateNames={
	"Enabled":"有效",
	"Disabled":"无效",
	"Deleted":"已删除",
}
// 
export interface SF$Sys$Services$Management$ServiceInstanceQueryArgument extends QueryArgument_ObjectKey_long {
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
export interface QueryArgument_ObjectKey_long {
	//
	//类型:ObjectKey_long
	Id?: ObjectKey_long;
	//
	//类型:SF.Sys.Entities.Paging
	Paging?: SF$Sys$Entities$Paging;
}
// 
export interface QueryResult_SF$Sys$Services$Management$Models$ServiceInstanceInternal {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Sys.Services.Management.Models.ServiceInstanceInternal[]
	Items?: SF$Sys$Services$Management$Models$ServiceInstanceInternal[];
}
// 
export interface QueryResult_ObjectKey_long {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:ObjectKey_long[]
	Items?: ObjectKey_long[];
}
// 
export interface SF$Sys$MenuServices$MenuItem extends UIObjectEntityBase_long {
	//字体图标
	//类型:string
	FontIcon?: string;
	//动作
	//类型:SF.Sys.MenuServices.MenuActionType
	Action: SF$Sys$MenuServices$MenuActionType;
	//动作参数
	//类型:string
	ActionArgument?: string;
	//服务
	//类型:long
	ServiceId?: number;
	//子菜单
	//类型:SF.Sys.MenuServices.MenuItem[]
	Children?: SF$Sys$MenuServices$MenuItem[];
	//
	//类型:long
	ParentId?: number;
	//
	//类型:int
	ItemOrder: number;
}
// 
export type SF$Sys$MenuServices$MenuActionType = 'None'|'EntityManager'|'Form'|'List'|'IFrame'|'Link';
export const SF$Sys$MenuServices$MenuActionTypeNames={
	"None":"无",
	"EntityManager":"实体管理",
	"Form":"显示表单",
	"List":"显示列表",
	"IFrame":"显示内嵌网页",
	"Link":"打开链接",
}
// 
export interface SF$Sys$MenuServices$Models$Menu extends ObjectEntityBase_long {
	//菜单引用标识
	//类型:string
	Ident: string;
}
// 
export interface SF$Sys$MenuServices$MenuQueryArgument extends QueryArgument_ObjectKey_long {
	//名称
	//类型:string
	Name?: string;
	//标识
	//类型:string
	Ident?: string;
}
// 
export interface QueryResult_SF$Sys$MenuServices$Models$Menu {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Sys.MenuServices.Models.Menu[]
	Items?: SF$Sys$MenuServices$Models$Menu[];
}
// 
export interface SF$Sys$MenuServices$Models$MenuEditable extends SF$Sys$MenuServices$Models$Menu {
	//菜单项
	//类型:SF.Sys.MenuServices.MenuItem[]
	Items?: SF$Sys$MenuServices$MenuItem[];
}
// 患者病历
export interface KDL$Services$Treatments$Patients$Models$Patient extends SF$Sys$Entities$DataModels$ObjectEntityBase {
}
// 
export interface SF$Sys$Entities$DataModels$ObjectEntityBase extends ObjectEntityBase_long {
}
// 
export interface KDL$Services$Treatments$Patients$PatientQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface SF$Sys$Entities$ObjectQueryArgument extends ObjectQueryArgument_ObjectKey_long {
}
// 
export interface ObjectQueryArgument_ObjectKey_long extends QueryArgument_ObjectKey_long {
	//名称
	//类型:string
	Name?: string;
	//逻辑状态
	//类型:SF.Sys.Entities.EntityLogicState
	LogicState?: SF$Sys$Entities$EntityLogicState;
}
// 
export interface QueryResult_KDL$Services$Treatments$Patients$Models$Patient {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Patients.Models.Patient[]
	Items?: KDL$Services$Treatments$Patients$Models$Patient[];
}
// 患者患病
export interface KDL$Services$Treatments$Patients$Models$PatientDisease {
	//患病
	//类型:long
	Id: number;
	//患病
	//类型:string
	DiseaseName?: string;
	//初诊时间
	//类型:datetime
	FirstDiagTime: string;
	//初诊医生
	//类型:long
	FirstDiagDoctorId: number;
	//初诊医生
	//类型:string
	FirstDiagDoctorName?: string;
	//最近复诊时间
	//类型:datetime
	LastDiagTime?: string;
	//最近复诊医生
	//类型:long
	LastDiagDoctorId: number;
	//最近复诊医生
	//类型:string
	LastDiagDoctorName?: string;
}
// 
export interface KDL$Services$Treatments$Patients$PatientDiseaseQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Patients$Models$PatientDisease {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Patients.Models.PatientDisease[]
	Items?: KDL$Services$Treatments$Patients$Models$PatientDisease[];
}
// 用药历史
export interface KDL$Services$Treatments$Patients$Models$PharmaconHistory extends SF$Sys$Entities$DataModels$ObjectEntityBase {
	//患者
	//类型:long
	PatientId: number;
	//患者
	//类型:string
	PatientName?: string;
	//药品
	//类型:long
	PharmaconId: number;
	//药品
	//类型:string
	PharmaconName?: string;
}
// 
export interface KDL$Services$Treatments$Patients$PharmaconHistoryQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Patients$Models$PharmaconHistory {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Patients.Models.PharmaconHistory[]
	Items?: KDL$Services$Treatments$Patients$Models$PharmaconHistory[];
}
// 服药计划
export interface KDL$Services$Treatments$Medications$Models$MedicationPlan {
	//ID
	//类型:long
	Id: number;
	//患者
	//类型:long
	PatientId: number;
	//患者
	//类型:string
	PatientName?: string;
	//药品
	//类型:long
	PharmaconId: number;
	//药品
	//类型:string
	PharmaconName?: string;
	//提醒计划
	//类型:long
	RemindPlanId: number;
	//提醒计划
	//类型:string
	RemindPlanName?: string;
	//服药开始日期
	//类型:datetime
	StartDate: string;
	//下次服药时间
	//类型:datetime
	NextTakeTime: string;
	//服药间隔
	//类型:int
	TakeIntervalDay: number;
	//一日服药次数
	//类型:int
	TakeTimesPerDay: number;
	//服药时间
	//类型:KDL.Services.Treatments.Medications.Models.MedicationTime[]
	TakeTimes: KDL$Services$Treatments$Medications$Models$MedicationTime[];
	//逻辑状态
	//类型:SF.Sys.Entities.EntityLogicState
	LogicState: SF$Sys$Entities$EntityLogicState;
	//
	//类型:KDL.Services.Treatments.Medications.Models.MedicineBoxCellSetting[]
	CellSettings?: KDL$Services$Treatments$Medications$Models$MedicineBoxCellSetting[];
}
// 服药时间
export interface KDL$Services$Treatments$Medications$Models$MedicationTime {
	//Id
	//类型:long
	Id: number;
	//
	//类型:timespan
	Time: string;
}
// 药槽设置
export interface KDL$Services$Treatments$Medications$Models$MedicineBoxCellSetting {
	//设备
	//类型:long
	DeviceId: number;
	//设备
	//类型:string
	DeviceName?: string;
	//药槽序号
	//类型:int
	Cell: number;
	//服药计划
	//类型:long
	PlanId: number;
	//服药计划
	//类型:string
	PlanName?: string;
}
// 
export interface KDL$Services$Treatments$Medications$MedicationPlanQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Medications$Models$MedicationPlan {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Medications.Models.MedicationPlan[]
	Items?: KDL$Services$Treatments$Medications$Models$MedicationPlan[];
}
// 服药记录
export interface KDL$Services$Treatments$Medications$Models$MedicationRecord extends SF$Sys$Entities$DataModels$EventEntityBase {
	//患者
	//类型:long
	PatientId: number;
	//患者
	//类型:string
	PatientName?: string;
	//服药计划
	//类型:long
	PlanId: number;
	//服药计划
	//类型:string
	PlanName?: string;
	//药品
	//类型:long
	PharmaconId: number;
	//药品
	//类型:string
	PharmaconName?: string;
	//计量单位
	//类型:long
	UnitId: number;
	//计量单位
	//类型:string
	UnitName?: string;
	//数量
	//类型:int
	Quantity: number;
	//服药状态
	//类型:KDL.Services.Treatments.Medications.Models.MedicationRecordState
	State: KDL$Services$Treatments$Medications$Models$MedicationRecordState;
}
// 
export interface SF$Sys$Entities$DataModels$EventEntityBase extends EventEntityBase_long {
}
// 
export interface EventEntityBase_long {
	//ID
	//类型:long
	Id: number;
	//区域
	//类型:long
	ScopeId?: number;
	//用户
	//类型:long
	UserId?: number;
	//时间
	//类型:datetime
	Time: string;
}
// 
export type KDL$Services$Treatments$Medications$Models$MedicationRecordState = 'Unexpect'|'OnTime'|'Delaied'|'Forgot'|'Unexpected';
export const KDL$Services$Treatments$Medications$Models$MedicationRecordStateNames={
	"Unexpect":"Unexpect",
	"OnTime":"OnTime",
	"Delaied":"Delaied",
	"Forgot":"Forgot",
	"Unexpected":"Unexpected",
}
// 
export interface KDL$Services$Treatments$Medications$MedicationRecordQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Medications$Models$MedicationRecord {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Medications.Models.MedicationRecord[]
	Items?: KDL$Services$Treatments$Medications$Models$MedicationRecord[];
}
// 
export interface KDL$Services$Treatments$Medications$MedicineBoxCellSettingQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Medications$Models$MedicineBoxCellSetting {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Medications.Models.MedicineBoxCellSetting[]
	Items?: KDL$Services$Treatments$Medications$Models$MedicineBoxCellSetting[];
}
// 药盒放药计划
export interface KDL$Services$Treatments$Medications$Models$MedicineBoxReloadPlan extends SF$Sys$Entities$DataModels$ObjectEntityBase {
	//药盒设备
	//类型:long
	DeviceId: number;
	//药盒
	//类型:string
	DeviceName?: string;
	//放药开始日期
	//类型:datetime
	StartDate: string;
	//放药间隔
	//类型:int
	IntervalDay: number;
	//放药时间
	//类型:timespan
	ReloadTime: string;
}
// 
export interface KDL$Services$Treatments$Medications$MedicineBoxReloadPlanQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Medications$Models$MedicineBoxReloadPlan {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Medications.Models.MedicineBoxReloadPlan[]
	Items?: KDL$Services$Treatments$Medications$Models$MedicineBoxReloadPlan[];
}
// 诊断记录
export interface KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecord extends SF$Sys$Entities$DataModels$EventEntityBase {
	//患者
	//类型:long
	PatientId: number;
	//患者
	//类型:string
	PatientName?: string;
	//医生
	//类型:long
	DoctorId: number;
	//医生
	//类型:string
	DoctorName?: string;
	//注释
	//类型:string
	Comment?: string;
	//项目
	//类型:KDL.Services.Treatments.Diagnostics.Models.DiagnosticRecordItem[]
	Items?: KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecordItem[];
}
// 诊断记录项目
export interface KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecordItem extends SF$Sys$Entities$DataModels$EventEntityBase {
	//患病
	//类型:long
	DiseaseId: number;
	//患病
	//类型:string
	DiseaseName?: string;
	//诊断结果
	//类型:KDL.Services.Treatments.Diagnostics.Models.DiagnosticResult
	Result: KDL$Services$Treatments$Diagnostics$Models$DiagnosticResult;
	//注释
	//类型:string
	Comment?: string;
}
// 
export type KDL$Services$Treatments$Diagnostics$Models$DiagnosticResult = 'Added'|'Continued'|'Recovered';
export const KDL$Services$Treatments$Diagnostics$Models$DiagnosticResultNames={
	"Added":"新增",
	"Continued":"持续",
	"Recovered":"恢复",
}
// 
export interface KDL$Services$Treatments$Diagnostics$DiagnosticRecordQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecord {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Diagnostics.Models.DiagnosticRecord[]
	Items?: KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecord[];
}
// 
export interface KDL$Services$Treatments$Diagnostics$DiagnosticRecordItemQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecordItem {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Diagnostics.Models.DiagnosticRecordItem[]
	Items?: KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecordItem[];
}
// 服药提醒计划
export interface KDL$Services$Treatments$Reminds$Models$RemindPlan extends ContainerEntityBase_KDL$Services$Treatments$Reminds$Models$RemindPlan_KDL$Services$Treatments$Reminds$Models$RemindAction {
}
// 
export interface ContainerEntityBase_KDL$Services$Treatments$Reminds$Models$RemindPlan_KDL$Services$Treatments$Reminds$Models$RemindAction extends ContainerEntityBase_KDL$Services$Treatments$Reminds$Models$RemindPlan_long_KDL$Services$Treatments$Reminds$Models$RemindAction_long_Nullable_long {
}
// 
export interface ContainerEntityBase_KDL$Services$Treatments$Reminds$Models$RemindPlan_long_KDL$Services$Treatments$Reminds$Models$RemindAction_long_Nullable_long extends ObjectEntityBase_long {
	//
	//类型:KDL.Services.Treatments.Reminds.Models.RemindAction[]
	Items?: KDL$Services$Treatments$Reminds$Models$RemindAction[];
}
// 服药提醒动作
export interface KDL$Services$Treatments$Reminds$Models$RemindAction extends ItemEntityBase_KDL$Services$Treatments$Reminds$Models$RemindPlan {
	//动作开始时间
	//类型:timespan
	ActionTime: string;
	//提醒目标
	//类型:KDL.Services.Treatments.Reminds.Models.RemindTarget
	Target: KDL$Services$Treatments$Reminds$Models$RemindTarget;
	//消息策略
	//类型:string
	MessagePolicy: string;
}
// 
export interface ItemEntityBase_KDL$Services$Treatments$Reminds$Models$RemindPlan extends ItemEntityBase_long_Nullable_long_KDL$Services$Treatments$Reminds$Models$RemindPlan {
}
// 
export interface ItemEntityBase_long_Nullable_long_KDL$Services$Treatments$Reminds$Models$RemindPlan extends ObjectEntityBase_long {
	//容器
	//类型:long
	ContainerId?: number;
	//排位
	//类型:int
	ItemOrder: number;
	//容器
	//类型:KDL.Services.Treatments.Reminds.Models.RemindPlan
	Container?: KDL$Services$Treatments$Reminds$Models$RemindPlan;
}
// 
export type KDL$Services$Treatments$Reminds$Models$RemindTarget = 'Patient'|'Friend';
export const KDL$Services$Treatments$Reminds$Models$RemindTargetNames={
	"Patient":"患者",
	"Friend":"好友",
}
// 
export interface KDL$Services$Treatments$Reminds$RemindPlanQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Reminds$Models$RemindPlan {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Reminds.Models.RemindPlan[]
	Items?: KDL$Services$Treatments$Reminds$Models$RemindPlan[];
}
// 
export interface KDL$Services$Treatments$Reminds$RemindActionQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Reminds$Models$RemindAction {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Reminds.Models.RemindAction[]
	Items?: KDL$Services$Treatments$Reminds$Models$RemindAction[];
}
// 服药提醒记录
export interface KDL$Services$Treatments$Reminds$Models$RemindRecord extends SF$Sys$Entities$DataModels$EventEntityBase {
	//患者
	//类型:long
	PatientId: number;
	//患者
	//类型:string
	PatientName?: string;
	//服药计划
	//类型:long
	PlanId: number;
	//服药计划
	//类型:string
	PlanName?: string;
	//药品
	//类型:long
	PharmaconId: number;
	//药品
	//类型:string
	PharmaconName?: string;
	//计量单位
	//类型:long
	UnitId: number;
	//计量单位
	//类型:string
	UnitName?: string;
	//数量
	//类型:int
	Quantity: number;
	//提醒计划
	//类型:long
	RemindPlanId: number;
	//提醒计划
	//类型:string
	RemindPlanName?: string;
	//提醒动作
	//类型:long
	RemindActionId: number;
	//提醒动作
	//类型:string
	RemindActionName?: string;
	//提醒对象
	//类型:long
	TargetId: number;
	//提醒对象
	//类型:string
	TargetName?: string;
	//提醒结果
	//类型:KDL.Services.Treatments.Reminds.Models.RemideResult
	Result: KDL$Services$Treatments$Reminds$Models$RemideResult;
}
// 
export type KDL$Services$Treatments$Reminds$Models$RemideResult = 'Success'|'Partial'|'Failed';
export const KDL$Services$Treatments$Reminds$Models$RemideResultNames={
	"Success":"成功",
	"Partial":"部分成功",
	"Failed":"失败",
}
// 
export interface KDL$Services$Treatments$Reminds$RemindRecordQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Treatments$Reminds$Models$RemindRecord {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Treatments.Reminds.Models.RemindRecord[]
	Items?: KDL$Services$Treatments$Reminds$Models$RemindRecord[];
}
// 设备
export interface KDL$Services$Devices$Models$Device extends SF$Sys$Entities$Models$ObjectEntityBase {
	//序列号
	//类型:string
	Sequence: string;
	//设备产品
	//类型:long
	ProductId: number;
	//设备产品
	//类型:string
	ProductName?: string;
	//设备型号
	//类型:long
	ModelId: number;
	//设备型号
	//类型:string
	ModelName?: string;
	//设备批次
	//类型:long
	BatchId: number;
	//设备类型
	//类型:string
	BatchName?: string;
	//
	//类型:string
	MAC?: string;
	//绑定用户
	//类型:long
	BindUserId?: number;
	//绑定用户
	//类型:string
	BindUserName?: string;
	//绑定时间
	//类型:datetime
	BindTime?: string;
	//最后访问地址
	//类型:string
	LastAccessAddress?: string;
	//最后访问时间
	//类型:datetime
	LastAccessTime?: string;
	//是否在线
	//类型:bool
	Online?: boolean;
}
// 
export interface SF$Sys$Entities$Models$ObjectEntityBase extends ObjectEntityBase_long {
}
// 
export interface KDL$Services$Devices$DeviceQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Devices$Models$Device {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Devices.Models.Device[]
	Items?: KDL$Services$Devices$Models$Device[];
}
// 设备型号
export interface KDL$Services$Devices$Models$DeviceModel extends SF$Sys$Entities$Models$ObjectEntityBase {
	//型号编号
	//类型:string
	Code: string;
	//设备产品
	//类型:long
	ProductId: number;
	//设备产品
	//类型:string
	ProductName?: string;
}
// 
export interface KDL$Services$Devices$DeviceModelQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Devices$Models$DeviceModel {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Devices.Models.DeviceModel[]
	Items?: KDL$Services$Devices$Models$DeviceModel[];
}
// 设备产品
export interface KDL$Services$Devices$Models$DeviceProduct extends SF$Sys$Entities$Models$UIObjectEntityBase {
	//产品编号
	//类型:string
	Code: string;
}
// 
export interface SF$Sys$Entities$Models$UIObjectEntityBase extends UIObjectEntityBase_long {
}
// 
export interface KDL$Services$Devices$DeviceProductQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
	//产品编号
	//类型:string
	Code?: string;
}
// 
export interface QueryResult_KDL$Services$Devices$Models$DeviceProduct {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Devices.Models.DeviceProduct[]
	Items?: KDL$Services$Devices$Models$DeviceProduct[];
}
// 设备批次
export interface KDL$Services$Devices$Models$DeviceBatch extends SF$Sys$Entities$Models$ObjectEntityBase {
	//批次编号
	//类型:string
	Code?: string;
	//设备产品
	//类型:long
	ProductId: number;
	//设备产品
	//类型:string
	ProductName?: string;
	//设备型号
	//类型:long
	ModelId: number;
	//设备型号
	//类型:string
	ModelName?: string;
	//设备数量
	//类型:int
	Count: number;
}
// 
export interface KDL$Services$Devices$DeviceBatchQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Devices$Models$DeviceBatch {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Devices.Models.DeviceBatch[]
	Items?: KDL$Services$Devices$Models$DeviceBatch[];
}
// 
export interface KDL$Services$Doctors$Models$Doctor extends SF$Sys$Entities$Models$ObjectEntityBase {
}
// 
export interface KDL$Services$Doctors$DoctorQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Doctors$Models$Doctor {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Doctors.Models.Doctor[]
	Items?: KDL$Services$Doctors$Models$Doctor[];
}
// 药品
export interface KDL$Services$Pharmacons$Models$Pharmacon extends UIItemEntityBase_KDL$Services$Pharmacons$Models$PharmaconCategory {
	//商品名
	//类型:string
	Title: string;
	//英文名称
	//类型:string
	EnName?: string;
	//生产商
	//类型:string
	Manufacturer?: string;
	//药品分类
	//类型:long
	CategoryId: number;
	//药品分类
	//类型:string
	CategoryName?: string;
	//药品规格
	//类型:KDL.Services.Pharmacons.Models.PharmaconSpec[]
	Specs?: KDL$Services$Pharmacons$Models$PharmaconSpec[];
}
// 
export interface UIItemEntityBase_KDL$Services$Pharmacons$Models$PharmaconCategory extends UIItemEntityBase_long_Nullable_long_KDL$Services$Pharmacons$Models$PharmaconCategory {
}
// 
export interface UIItemEntityBase_long_Nullable_long_KDL$Services$Pharmacons$Models$PharmaconCategory extends UIObjectEntityBase_long {
	//容器ID
	//类型:long
	ContainerId?: number;
	//容器名
	//类型:string
	ContainerName?: string;
	//容器
	//类型:KDL.Services.Pharmacons.Models.PharmaconCategory
	Container?: KDL$Services$Pharmacons$Models$PharmaconCategory;
	//排位
	//类型:int
	ItemOrder?: number;
}
// 
export interface KDL$Services$Pharmacons$Models$PharmaconCategory extends ContainerEntityBase_KDL$Services$Pharmacons$Models$Pharmacon {
}
// 
export interface ContainerEntityBase_KDL$Services$Pharmacons$Models$Pharmacon extends ContainerEntityBase_long_KDL$Services$Pharmacons$Models$Pharmacon_Nullable_long {
}
// 
export interface ContainerEntityBase_long_KDL$Services$Pharmacons$Models$Pharmacon_Nullable_long extends ObjectEntityBase_long {
	//子对象
	//类型:KDL.Services.Pharmacons.Models.Pharmacon[]
	Items?: KDL$Services$Pharmacons$Models$Pharmacon[];
}
// 药品规格
export interface KDL$Services$Pharmacons$Models$PharmaconSpec extends SF$Sys$Entities$Models$ObjectEntityBase {
	//规格名称
	//类型:string
	Name: string;
	//条形码
	//类型:string
	Code?: string;
	//药品
	//类型:long
	MedicineId: number;
	//药品名称
	//类型:string
	MedicineName?: string;
	//包装
	//类型:string
	Package?: string;
}
// 
export interface KDL$Services$Pharmacons$PharmaconQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
	//文档分类
	//类型:int
	CategoryId?: number;
	//英文名
	//类型:string
	EnName?: string;
	//商品名
	//类型:string
	Title?: string;
}
// 
export interface QueryResult_KDL$Services$Pharmacons$Models$Pharmacon {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Pharmacons.Models.Pharmacon[]
	Items?: KDL$Services$Pharmacons$Models$Pharmacon[];
}
// 
export interface KDL$Services$Pharmacons$PharmaconSpecQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Pharmacons$Models$PharmaconSpec {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Pharmacons.Models.PharmaconSpec[]
	Items?: KDL$Services$Pharmacons$Models$PharmaconSpec[];
}
// 
export interface KDL$Services$Pharmacons$PharmaconCategoryQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
	//父分类
	//类型:int
	ContainerId?: number;
}
// 
export interface QueryResult_KDL$Services$Pharmacons$Models$PharmaconCategory {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Pharmacons.Models.PharmaconCategory[]
	Items?: KDL$Services$Pharmacons$Models$PharmaconCategory[];
}
// 计量单位
export interface KDL$Services$Pharmacons$Models$PharmaconUnit extends SF$Sys$Entities$Models$ObjectEntityBase {
}
// 
export interface KDL$Services$Pharmacons$PharmaconUnitQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Pharmacons$Models$PharmaconUnit {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Pharmacons.Models.PharmaconUnit[]
	Items?: KDL$Services$Pharmacons$Models$PharmaconUnit[];
}
// 疾病
export interface KDL$Services$Diseases$Models$Disease extends TreeNodeEntityBase_KDL$Services$Diseases$Models$Disease {
}
// 
export interface TreeNodeEntityBase_KDL$Services$Diseases$Models$Disease extends TreeNodeEntityBase_KDL$Services$Diseases$Models$Disease_long_Nullable_long {
}
// 
export interface TreeNodeEntityBase_KDL$Services$Diseases$Models$Disease_long_Nullable_long extends ItemEntityBase_long_Nullable_long_KDL$Services$Diseases$Models$Disease {
	//子节点
	//类型:KDL.Services.Diseases.Models.Disease[]
	Children?: KDL$Services$Diseases$Models$Disease[];
}
// 
export interface ItemEntityBase_long_Nullable_long_KDL$Services$Diseases$Models$Disease extends ObjectEntityBase_long {
	//容器
	//类型:long
	ContainerId?: number;
	//容器
	//类型:string
	ContainerName?: string;
	//容器
	//类型:KDL.Services.Diseases.Models.Disease
	Container?: KDL$Services$Diseases$Models$Disease;
	//排位
	//类型:int
	ItemOrder?: number;
}
// 
export interface KDL$Services$Diseases$DiseaseQueryArgument extends SF$Sys$Entities$ObjectQueryArgument {
}
// 
export interface QueryResult_KDL$Services$Diseases$Models$Disease {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:KDL.Services.Diseases.Models.Disease[]
	Items?: KDL$Services$Diseases$Models$Disease[];
}
// 凭证类型
export interface SF$Auth$IdentityServices$Models$ClaimType extends ObjectEntityBase_string {
	//
	//类型:string
	Id?: string;
}
// 
export interface ObjectEntityBase_string {
	//Id
	//类型:string
	Id?: string;
	//名称
	//类型:string
	Name: string;
	//对象状态
	//类型:SF.Sys.Entities.EntityLogicState
	LogicState: SF$Sys$Entities$EntityLogicState;
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
export interface SF$Auth$IdentityServices$Managers$ClaimTypeQueryArgument extends ObjectQueryArgument_ObjectKey_string {
}
// 
export interface ObjectQueryArgument_ObjectKey_string extends QueryArgument_ObjectKey_string {
	//名称
	//类型:string
	Name?: string;
	//逻辑状态
	//类型:SF.Sys.Entities.EntityLogicState
	LogicState?: SF$Sys$Entities$EntityLogicState;
}
// 
export interface QueryResult_SF$Auth$IdentityServices$Models$ClaimType {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Auth.IdentityServices.Models.ClaimType[]
	Items?: SF$Auth$IdentityServices$Models$ClaimType[];
}
// 认证客户端配置
export interface SF$Auth$IdentityServices$Models$ClientConfigEditable extends SF$Auth$IdentityServices$Models$ClientConfigInternal {
	//发送用户声明
	//类型:bool
	AlwaysIncludeUserClaimsInIdToken?: boolean;
	//身份令牌超时
	//类型:int
	IdentityTokenLifetime: number;
	//访问令牌超时
	//类型:int
	AccessTokenLifetime: number;
	//授权码超时
	//类型:int
	AuthorizationCodeLifetime: number;
	//更新令牌绝对超时
	//类型:int
	AbsoluteRefreshTokenLifetime: number;
	//更新令牌间隔超时
	//类型:int
	SlidingRefreshTokenLifetime: number;
	//授权超时
	//类型:int
	ConsentLifetime?: number;
	//客户端声明前缀
	//类型:string
	ClientClaimsPrefix?: string;
	//允许请求范围
	//类型:SF.Auth.IdentityServices.Models.ClientScope[]
	Scopes?: SF$Auth$IdentityServices$Models$ClientScope[];
	//离线访问
	//类型:bool
	AllowOfflineAccess?: boolean;
	//客户端密钥
	//类型:string
	ClientSecrets?: string;
	//是否需要密钥
	//类型:bool
	RequireClientSecret?: boolean;
	//跨域设置
	//类型:string
	AllowedCorsOrigins?: string;
	//是否需要授权
	//类型:bool
	RequireConsent?: boolean;
	//授权类型
	//类型:string[]
	AllowedGrantTypes?: string[];
	//需要注销会话
	//类型:bool
	FrontChannelLogoutSessionRequired?: boolean;
	//是否记住授权
	//类型:bool
	AllowRememberConsent?: boolean;
	//是否需要注销会话
	//类型:bool
	BackChannelLogoutSessionRequired?: boolean;
}
// 认证客户端配置
export interface SF$Auth$IdentityServices$Models$ClientConfigInternal extends ObjectEntityBase_long {
}
// 
export interface SF$Auth$IdentityServices$Models$ClientScope {
	//范围
	//类型:string
	ScopeId?: string;
	//范围
	//类型:string
	ScopeName?: string;
}
// 
export interface SF$Auth$IdentityServices$Managers$ClientConfigQueryArgument extends ObjectQueryArgument_ObjectKey_long {
}
// 
export interface QueryResult_SF$Auth$IdentityServices$Models$ClientConfigInternal {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Auth.IdentityServices.Models.ClientConfigInternal[]
	Items?: SF$Auth$IdentityServices$Models$ClientConfigInternal[];
}
// 认证客户端
export interface SF$Auth$IdentityServices$Models$ClientEditable extends SF$Auth$IdentityServices$Models$ClientInternal {
	//客户端Url
	//类型:string
	ClientUri?: string;
	//注销跳转地址
	//类型:string
	PostLogoutRedirectUris?: string;
	//前端注销跳转地址
	//类型:string
	FrontChannelLogoutUri?: string;
	//登录跳转地址
	//类型:string
	RedirectUris?: string;
}
// 认证客户端
export interface SF$Auth$IdentityServices$Models$ClientInternal extends UIObjectEntityBase_string {
	//配置
	//类型:long
	ClientConfigId: number;
	//配置
	//类型:string
	ClientConfigName?: string;
	//客户端密钥
	//类型:string
	Secret?: string;
}
// 
export interface UIObjectEntityBase_string extends ObjectEntityBase_string {
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
export interface SF$Auth$IdentityServices$Managers$ClientQueryArgument extends QueryArgument_ObjectKey_string {
	//客户端名称
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Auth$IdentityServices$Models$ClientInternal {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Auth.IdentityServices.Models.ClientInternal[]
	Items?: SF$Auth$IdentityServices$Models$ClientInternal[];
}
// 操作范围
export interface SF$Auth$IdentityServices$Models$OperationInternal extends UIObjectEntityBase_string {
}
// 
export interface SF$Auth$IdentityServices$Managers$OperationQueryArgument extends ObjectQueryArgument_ObjectKey_string {
}
// 
export interface QueryResult_SF$Auth$IdentityServices$Models$OperationInternal {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Auth.IdentityServices.Models.OperationInternal[]
	Items?: SF$Auth$IdentityServices$Models$OperationInternal[];
}
// 资源
export interface SF$Auth$IdentityServices$Models$ResourceEditable extends SF$Auth$IdentityServices$Models$ResourceInternal {
	//可用操作
	//类型:SF.Auth.IdentityServices.Models.ResourceOperationInternal[]
	SupportedOperations?: SF$Auth$IdentityServices$Models$ResourceOperationInternal[];
	//所需申明
	//类型:SF.Auth.IdentityServices.Models.ResourceRequiredClaim[]
	RequiredClaims?: SF$Auth$IdentityServices$Models$ResourceRequiredClaim[];
}
// 资源
export interface SF$Auth$IdentityServices$Models$ResourceInternal extends UIObjectEntityBase_string {
	//
	//类型:string
	Id?: string;
	//标识资源
	//类型:bool
	IsIdentityResource?: boolean;
}
// 资源操作
export interface SF$Auth$IdentityServices$Models$ResourceOperationInternal {
	//操作
	//类型:string
	OperationId?: string;
	//操作名称
	//类型:string
	OperationName?: string;
}
// 
export interface SF$Auth$IdentityServices$Models$ResourceRequiredClaim {
	//申明类型
	//类型:string
	ClaimTypeId: string;
	//申明类型
	//类型:string
	ClaimTypeName?: string;
}
// 
export interface SF$Auth$IdentityServices$Managers$ResourceQueryArgument extends ObjectQueryArgument_ObjectKey_string {
}
// 
export interface QueryResult_SF$Auth$IdentityServices$Models$ResourceInternal {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Auth.IdentityServices.Models.ResourceInternal[]
	Items?: SF$Auth$IdentityServices$Models$ResourceInternal[];
}
// 角色
export interface SF$Auth$IdentityServices$Models$RoleEditable extends SF$Auth$IdentityServices$Models$Role {
	//
	//类型:SF.Auth.IdentityServices.Models.ClaimValue[]
	Claims?: SF$Auth$IdentityServices$Models$ClaimValue[];
	//
	//类型:SF.Auth.IdentityServices.Models.Grant[]
	Grants?: SF$Auth$IdentityServices$Models$Grant[];
}
// 角色
export interface SF$Auth$IdentityServices$Models$Role extends ObjectEntityBase_string {
}
// 凭证参数值
export interface SF$Auth$IdentityServices$Models$ClaimValue {
	//类型ID
	//类型:string
	TypeId?: string;
	//类型
	//类型:string
	TypeName?: string;
	//凭证值
	//类型:string
	Value?: string;
	//发行时间
	//类型:datetime
	IssueTime: string;
}
// 
export interface SF$Auth$IdentityServices$Models$Grant {
	//操作资源ID
	//类型:string
	ResourceId?: string;
	//
	//类型:string
	ResourceName?: string;
	//操作区域ID
	//类型:string
	OperationId?: string;
	//
	//类型:string
	OperationName?: string;
}
// 
export interface SF$Auth$IdentityServices$Managers$RoleQueryArgument extends ObjectQueryArgument_ObjectKey_string {
}
// 
export interface QueryResult_SF$Auth$IdentityServices$Models$Role {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Auth.IdentityServices.Models.Role[]
	Items?: SF$Auth$IdentityServices$Models$Role[];
}
// 
export interface SF$Auth$IdentityServices$Models$UserEditable extends SF$Auth$IdentityServices$Models$UserInternal {
	//图标
	//类型:string
	Icon?: string;
	//头像
	//类型:string
	Image?: string;
	//密码
	//类型:string
	PasswordHash?: string;
	//安全标识
	//类型:string
	SecurityStamp?: string;
	//登录凭证
	//类型:SF.Auth.IdentityServices.Models.UserCredential[]
	Credentials?: SF$Auth$IdentityServices$Models$UserCredential[];
	//附加参数
	//类型:string{}
	SignupExtraArgument?: {[index:string]:string};
	//申明
	//类型:SF.Auth.IdentityServices.Models.ClaimValue[]
	Claims?: SF$Auth$IdentityServices$Models$ClaimValue[];
	//角色
	//类型:SF.Auth.IdentityServices.Models.UserRole[]
	Roles?: SF$Auth$IdentityServices$Models$UserRole[];
	//客户端
	//类型:long
	SignupClientId?: number;
	//客户端
	//类型:string
	SignupClientName?: string;
}
// 
export interface SF$Auth$IdentityServices$Models$UserInternal extends SF$Sys$Entities$Models$ObjectEntityBase {
	//创建标识
	//类型:string
	MainCredential: string;
	//创建标识类型
	//类型:string
	MainClaimTypeId: string;
	//创建标识类型
	//类型:string
	MainClaimTypeName?: string;
}
// 
export interface SF$Auth$IdentityServices$Models$UserCredential {
	//类型
	//类型:string
	ClaimTypeId?: string;
	//
	//类型:long
	UserId: number;
	//凭证值
	//类型:string
	Credential?: string;
	//创建时间
	//类型:datetime
	CreatedTime: string;
	//确认时间
	//类型:datetime
	ConfirmedTime?: string;
}
// 
export interface SF$Auth$IdentityServices$Models$UserRole {
	//类型ID
	//类型:string
	RoleId?: string;
	//类型
	//类型:string
	RoleName?: string;
}
// 
export interface SF$Auth$IdentityServices$Managers$UserQueryArgument extends ObjectQueryArgument_ObjectKey_long {
	//注册账号
	//类型:string
	MainCredential?: string;
	//注册类型
	//类型:string
	MainClaimTypeId?: string;
	//姓名
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Auth$IdentityServices$Models$UserInternal {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Auth.IdentityServices.Models.UserInternal[]
	Items?: SF$Auth$IdentityServices$Models$UserInternal[];
}
// 
export interface SF$Auth$IdentityServices$Internals$UserCreateArgument {
	//用户信息
	//类型:SF.Sys.Auth.User
	User: SF$Sys$Auth$User;
	//密码哈希
	//类型:string
	PasswordHash: string;
	//安全戳
	//类型:byte[]
	SecurityStamp: number[];
	//访问源
	//类型:SF.Sys.Clients.IUserAgent
	UserAgent: SF$Sys$Clients$IUserAgent;
	//登录凭证
	//类型:string
	CredentialValue: string;
	//登录凭证提供者
	//类型:string
	ClaimTypeId?: string;
	//用户角色
	//类型:string[]
	Roles?: string[];
	//用户申明
	//类型:SF.Auth.IdentityServices.Models.ClaimValue[]
	ClaimValues?: SF$Auth$IdentityServices$Models$ClaimValue[];
	//注册附加参数
	//类型:string{}
	ExtraArgument?: {[index:string]:string};
}
// 身份标识
export interface SF$Sys$Auth$User {
	//ID
	//类型:long
	Id: number;
	//名称
	//类型:string
	Name: string;
	//图标
	//类型:string
	Icon?: string;
}
// 
export interface SF$Sys$Clients$IUserAgent {
	//
	//类型:string{}
	ExtraValues?: {[index:string]:string};
	//
	//类型:string
	Address?: string;
	//
	//类型:string
	AgentName?: string;
	//
	//类型:SF.Sys.Clients.ClientDeviceType
	DeviceType: SF$Sys$Clients$ClientDeviceType;
}
// 
export type SF$Sys$Clients$ClientDeviceType = 'PCDesktop'|'PCBrowser'|'WinXin'|'Andriod'|'iPhone'|'WAP'|'Console';
export const SF$Sys$Clients$ClientDeviceTypeNames={
	"PCDesktop":"PCDesktop",
	"PCBrowser":"PCBrowser",
	"WinXin":"WinXin",
	"Andriod":"Andriod",
	"iPhone":"iPhone",
	"WAP":"WAP",
	"Console":"Console",
}
// 用户身份数据
export interface SF$Auth$IdentityServices$Internals$UserData {
	//
	//类型:long
	Id: number;
	//
	//类型:byte[]
	SecurityStamp?: number[];
	//
	//类型:string
	PasswordHash?: string;
	//
	//类型:bool
	IsEnabled?: boolean;
	//
	//类型:string
	Name?: string;
	//
	//类型:string
	Icon?: string;
	//
	//类型:SF.Auth.IdentityServices.Models.ClaimValue[]
	Claims?: SF$Auth$IdentityServices$Models$ClaimValue[];
	//
	//类型:string[]
	Roles?: string[];
}
// 授权范围
export interface SF$Auth$IdentityServices$Models$ScopeEditable extends SF$Auth$IdentityServices$Models$ScopeInternal {
	//授权资源
	//类型:string[]
	Resources?: string[];
}
// 授权范围
export interface SF$Auth$IdentityServices$Models$ScopeInternal extends ObjectEntityBase_string {
}
// 
export interface SF$Auth$IdentityServices$Managers$ScopeQueryArgument extends ObjectQueryArgument_ObjectKey_string {
}
// 
export interface QueryResult_SF$Auth$IdentityServices$Models$ScopeInternal {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Auth.IdentityServices.Models.ScopeInternal[]
	Items?: SF$Auth$IdentityServices$Models$ScopeInternal[];
}
// 
export interface SF$Auth$IdentityServices$SigninArgument {
	//用户
	//类型:string
	Ident?: string;
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
	//客户端ID
	//类型:string
	ClientId?: string;
}
// 
export interface SF$Auth$IdentityServices$SendPasswordRecorveryCodeArgument {
	//身份验证服务
	//类型:string
	CredentialProvider?: string;
	//人工操作验证码
	//类型:string
	CaptchaCode?: string;
	//用户
	//类型:string
	Credential?: string;
}
// 
export interface SF$Auth$IdentityServices$ResetPasswordByRecorveryCodeArgument {
	//身份验证服务
	//类型:string
	CredentialProvider?: string;
	//用户
	//类型:string
	Credential?: string;
	//验证码
	//类型:string
	VerifyCode?: string;
	//新密码
	//类型:string
	NewPassword?: string;
	//客户端ID
	//类型:string
	ClientId?: string;
	//是否返回身份令牌
	//类型:bool
	ReturnToken?: boolean;
}
// 
export interface SF$Auth$IdentityServices$SetPasswordArgument {
	//就密码
	//类型:string
	OldPassword?: string;
	//新密码
	//类型:string
	NewPassword?: string;
	//客户端ID
	//类型:string
	ClientId?: string;
	//是否返回身份令牌
	//类型:bool
	ReturnToken?: boolean;
}
// 
export interface SF$Auth$IdentityServices$SendCreateIdentityVerifyCodeArgument {
	//身份验证服务
	//类型:string
	CredentialProvider?: string;
	//人工操作验证码
	//类型:string
	Credetial?: string;
	//人工操作验证码
	//类型:string
	CaptchaCode?: string;
}
// 
export interface SF$Auth$IdentityServices$SignupArgument {
	//身份验证服务ID
	//类型:string
	CredentialProvider?: string;
	//身份信息
	//类型:SF.Sys.Auth.User
	User?: SF$Sys$Auth$User;
	//用户
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
	//客户端ID
	//类型:string
	ClientId?: string;
	//过期时间
	//类型:int
	Expires?: number;
	//
	//类型:string[]
	Roles?: string[];
	//附加参数
	//类型:string{}
	ExtraArgument?: {[index:string]:string};
}
// 
export interface SF$Common$TextMessages$Models$MsgRecord extends SF$Sys$Entities$Models$EventEntityBase {
	//消息名称
	//类型:string
	Name?: string;
	//状态
	//类型:SF.Common.TextMessages.Models.SendStatus
	Status: SF$Common$TextMessages$Models$SendStatus;
	//消息策略
	//类型:long
	PolicyId: number;
	//消息策略
	//类型:string
	PolicyName?: string;
	//消息参数
	//类型:string
	Args?: string;
	//开始时间
	//类型:datetime
	Time: string;
	//结束时间
	//类型:datetime
	EndTime?: string;
	//跟踪对象
	//类型:string
	TrackEntityId?: string;
	//所有动作
	//类型:int
	ActionCount: number;
	//完成动作
	//类型:int
	CompletedActionCount: number;
	//动作记录
	//类型:SF.Common.TextMessages.Models.MsgActionRecord[]
	ActionRecords?: SF$Common$TextMessages$Models$MsgActionRecord[];
}
// 
export interface SF$Sys$Entities$Models$EventEntityBase extends EventEntityBase_long {
}
// 
export type SF$Common$TextMessages$Models$SendStatus = 'Sending'|'Success'|'Failed';
export const SF$Common$TextMessages$Models$SendStatusNames={
	"Sending":"发送中",
	"Success":"发送成功",
	"Failed":"发送失败",
}
// 
export interface SF$Common$TextMessages$Models$MsgActionRecord extends SF$Sys$Entities$Models$EventEntityBase {
	//状态
	//类型:SF.Common.TextMessages.Models.SendStatus
	Status: SF$Common$TextMessages$Models$SendStatus;
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
	Content?: string;
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
	//消息记录
	//类型:long
	MsgRecordId: number;
	//消息记录
	//类型:string
	MsgRecordName?: string;
}
// 
export interface SF$Common$TextMessages$Management$MsgRecordQueryArgument extends QueryArgument_ObjectKey_long {
	//目标用户
	//类型:long
	TargeUserId?: number;
	//发送时间
	//类型:QueryRange_datetime
	Time?: QueryRange_datetime;
	//消息策略
	//类型:long
	PolicyId?: number;
	//发送服务
	//类型:long
	ServiceId?: number;
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
export interface QueryResult_SF$Common$TextMessages$Models$MsgRecord {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.TextMessages.Models.MsgRecord[]
	Items?: SF$Common$TextMessages$Models$MsgRecord[];
}
// 
export interface SF$Common$TextMessages$Management$MsgActionRecordQueryArgument extends QueryArgument_ObjectKey_long {
	//状态
	//类型:SF.Common.TextMessages.Models.SendStatus
	Status?: SF$Common$TextMessages$Models$SendStatus;
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
	//消息策略
	//类型:long
	PolicyId?: number;
}
// 
export interface QueryResult_SF$Common$TextMessages$Models$MsgActionRecord {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.TextMessages.Models.MsgActionRecord[]
	Items?: SF$Common$TextMessages$Models$MsgActionRecord[];
}
// 
export interface SF$Common$TextMessages$Models$MsgPolicy extends SF$Sys$Entities$Models$ObjectEntityBase {
	//策略标识
	//类型:string
	Ident: string;
	//发送动作
	//类型:SF.Common.TextMessages.Models.MessageSendAction[]
	Actions?: SF$Common$TextMessages$Models$MessageSendAction[];
}
// 
export interface SF$Common$TextMessages$Models$MessageSendAction {
	//发送多种名称
	//类型:string
	Name?: string;
	//是否禁用此动作
	//类型:bool
	Disabled?: boolean;
	//错误重发次数
	//类型:int
	RetryCount: number;
	//错误重发间隔
	//类型:int
	RetryInterval: number;
	//消息发送服务
	//类型:long
	ProviderId: number;
	//发送目标模板
	//类型:string
	TargetTemplate?: string;
	//标题模板
	//类型:string
	TitleTemplate?: string;
	//内容模板
	//类型:string
	ContentTemplate?: string;
	//外部模板ID
	//类型:string
	ExtTemplateId?: string;
	//外部模板参数
	//类型:SF.Common.TextMessages.Models.ExtTemplateArgument[]
	ExtTemplateArguments?: SF$Common$TextMessages$Models$ExtTemplateArgument[];
}
// 
export interface SF$Common$TextMessages$Models$ExtTemplateArgument {
	//参数名称
	//类型:string
	Name?: string;
	//参数内容
	//类型:string
	Value?: string;
}
// 
export interface SF$Common$TextMessages$Management$MsgPolicyQueryArgument extends QueryArgument_ObjectKey_long {
	//消息策略
	//类型:string
	Ident?: string;
}
// 
export interface QueryResult_SF$Common$TextMessages$Models$MsgPolicy {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.TextMessages.Models.MsgPolicy[]
	Items?: SF$Common$TextMessages$Models$MsgPolicy[];
}
// 会员
export interface SF$Common$Members$Models$MemberInternal extends SF$Common$Members$Models$Member {
}
// 会员
export interface SF$Common$Members$Models$Member extends ObjectEntityBase_long {
}
// 
export interface SF$Common$Members$MemberQueryArgument extends QueryArgument_ObjectKey_long {
	//
	//类型:string
	Ident?: string;
	//
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Common$Members$Models$MemberInternal {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.Members.Models.MemberInternal[]
	Items?: SF$Common$Members$Models$MemberInternal[];
}
// 
export interface QueryResult_long {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:long[]
	Items?: number[];
}
// 会员
export interface SF$Common$Members$Models$MemberEditable extends SF$Common$Members$Models$MemberInternal {
}
// 
export interface SF$Common$Admins$Models$AdminInternal extends SF$Sys$Entities$Models$UIObjectEntityBase {
}
// 
export interface SF$Common$Admins$AdminQueryArgument extends SF$Sys$Entities$QueryArgument {
	//
	//类型:string
	Account?: string;
}
// 
export interface SF$Sys$Entities$QueryArgument extends QueryArgument_ObjectKey_long {
}
// 
export interface QueryResult_SF$Common$Admins$Models$AdminInternal {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.Admins.Models.AdminInternal[]
	Items?: SF$Common$Admins$Models$AdminInternal[];
}
// 
export interface SF$Common$Admins$Models$AdminEditable extends SF$Common$Admins$Models$AdminInternal {
}
// 
export interface SF$Common$FrontEndContents$Site {
	//Id
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
export interface QueryResult_SF$Common$FrontEndContents$Site {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.FrontEndContents.Site[]
	Items?: SF$Common$FrontEndContents$Site[];
}
// 
export interface SF$Common$FrontEndContents$SiteTemplate {
	//ID
	//类型:long
	Id: number;
	//名称
	//类型:string
	Name: string;
	//
	//类型:SF.Common.FrontEndContents.SiteConfigModels.SiteModel
	Model: SF$Common$FrontEndContents$SiteConfigModels$SiteModel;
}
// 
export interface SF$Common$FrontEndContents$SiteConfigModels$SiteModel {
	//名称
	//类型:string
	name?: string;
	//页面列表
	//类型:SF.Common.FrontEndContents.SiteConfigModels.PageModel[]
	pages?: SF$Common$FrontEndContents$SiteConfigModels$PageModel[];
}
// 
export interface SF$Common$FrontEndContents$SiteConfigModels$PageModel {
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
	//类型:SF.Common.FrontEndContents.SiteConfigModels.BlockModel[]
	blocks?: SF$Common$FrontEndContents$SiteConfigModels$BlockModel[];
}
// 
export interface SF$Common$FrontEndContents$SiteConfigModels$BlockModel {
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
	//类型:SF.Common.FrontEndContents.SiteConfigModels.BlockContentModel[]
	contents?: SF$Common$FrontEndContents$SiteConfigModels$BlockContentModel[];
}
// 
export interface SF$Common$FrontEndContents$SiteConfigModels$BlockContentModel {
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
export interface SF$Common$FrontEndContents$SiteTemplateQueryArgument extends QueryArgument_ObjectKey_long {
	//
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Common$FrontEndContents$SiteTemplate {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.FrontEndContents.SiteTemplate[]
	Items?: SF$Common$FrontEndContents$SiteTemplate[];
}
// 
export interface SF$Common$FrontEndContents$Content extends SF$Common$FrontEndContents$ContentItem {
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
export interface SF$Common$FrontEndContents$ContentItem {
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
	//类型:SF.Common.FrontEndContents.ContentItem[]
	Items?: SF$Common$FrontEndContents$ContentItem[];
}
// 
export interface SF$Common$FrontEndContents$ContentQueryArgument extends QueryArgument_Option_long {
	//
	//类型:string
	Category?: string;
	//
	//类型:string
	Name?: string;
}
// 
export interface QueryArgument_Option_long {
	//
	//类型:long
	Id?: number;
	//
	//类型:SF.Sys.Entities.Paging
	Paging?: SF$Sys$Entities$Paging;
}
// 
export interface QueryResult_SF$Common$FrontEndContents$Content {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.FrontEndContents.Content[]
	Items?: SF$Common$FrontEndContents$Content[];
}
// 
export interface SF$Common$FrontEndContents$IContent extends SF$Common$FrontEndContents$IContentItem {
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
export interface SF$Common$FrontEndContents$IContentItem {
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
	//类型:SF.Common.FrontEndContents.ContentItem[]
	Items?: SF$Common$FrontEndContents$ContentItem[];
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
	//子项目
	//类型:SF.Common.Documents.Management.DocumentInternal[]
	Items?: SF$Common$Documents$Management$DocumentInternal[];
}
// 
export interface UITreeNodeEntityBase_SF$Common$Documents$Management$CategoryInternal_long_Nullable_long extends UIItemEntityBase_long_Nullable_long_SF$Common$Documents$Management$CategoryInternal {
	//子节点
	//类型:SF.Common.Documents.Management.CategoryInternal[]
	Children?: SF$Common$Documents$Management$CategoryInternal[];
}
// 
export interface UIItemEntityBase_long_Nullable_long_SF$Common$Documents$Management$CategoryInternal extends UIObjectEntityBase_long {
	//容器ID
	//类型:long
	ContainerId?: number;
	//容器名
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
	//分类名称
	//类型:string
	ContainerName?: string;
}
// 
export interface UIItemEntityBase_SF$Common$Documents$Management$CategoryInternal extends UIItemEntityBase_long_Nullable_long_SF$Common$Documents$Management$CategoryInternal {
}
// 
export interface SF$Common$Documents$Management$DocumentCategoryQueryArgument extends SF$Sys$Entities$QueryArgument {
	//父分类
	//类型:int
	ParentId?: number;
	//名称
	//类型:string
	Name?: string;
}
// 
export interface QueryResult_SF$Common$Documents$Management$CategoryInternal {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.Documents.Management.CategoryInternal[]
	Items?: SF$Common$Documents$Management$CategoryInternal[];
}
// 
export interface SF$Common$Documents$Management$DocumentQueryArguments extends SF$Sys$Entities$QueryArgument {
	//文档分类
	//类型:int
	CategoryId?: number;
	//标题
	//类型:string
	Name?: string;
	//发布日期
	//类型:SF.Sys.Entities.NullableDateQueryRange
	PublishDate?: SF$Sys$Entities$NullableDateQueryRange;
}
// 
export interface SF$Sys$Entities$NullableDateQueryRange extends NullableQueryRange_datetime {
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
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
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
// 文档实体
export interface SF$Common$Documents$Document extends UIItemEntityBase_SF$Common$Documents$Category {
	//文档内容,Html格式
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
	//容器名
	//类型:string
	ContainerName?: string;
	//容器
	//类型:SF.Common.Documents.Category
	Container?: SF$Common$Documents$Category;
	//排位
	//类型:int
	ItemOrder?: number;
}
// 文档目录实体
export interface SF$Common$Documents$Category extends UITreeContainerEntityBase_SF$Common$Documents$Category_SF$Common$Documents$Document {
	//子目录
	//类型:SF.Common.Documents.Category[]
	Children?: SF$Common$Documents$Category[];
	//目录中的文档
	//类型:SF.Common.Documents.Document[]
	Items?: SF$Common$Documents$Document[];
}
// 
export interface UITreeContainerEntityBase_SF$Common$Documents$Category_SF$Common$Documents$Document extends UITreeContainerEntityBase_SF$Common$Documents$Category_long_Nullable_long_SF$Common$Documents$Document_Nullable_long {
}
// 
export interface UITreeContainerEntityBase_SF$Common$Documents$Category_long_Nullable_long_SF$Common$Documents$Document_Nullable_long extends UITreeNodeEntityBase_SF$Common$Documents$Category_long_Nullable_long {
	//子项目
	//类型:SF.Common.Documents.Document[]
	Items?: SF$Common$Documents$Document[];
}
// 
export interface UITreeNodeEntityBase_SF$Common$Documents$Category_long_Nullable_long extends UIItemEntityBase_long_Nullable_long_SF$Common$Documents$Category {
	//子节点
	//类型:SF.Common.Documents.Category[]
	Children?: SF$Common$Documents$Category[];
}
// 分页配置
export interface SF$Sys$Entities$SearchArgument extends SF$Sys$Entities$PagingArgument {
	//搜索关键字
	//类型:string
	Key?: string;
}
// 分页配置
export interface SF$Sys$Entities$PagingArgument {
	//分页参数
	//类型:SF.Sys.Entities.Paging
	Paging?: SF$Sys$Entities$Paging;
}
// 
export interface QueryResult_SF$Common$Documents$Document {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.Documents.Document[]
	Items?: SF$Common$Documents$Document[];
}
//  分页配置
export interface ListItemsArgument_Nullable_long extends SF$Sys$Entities$PagingArgument {
	//容器对象ID
	//类型:long
	Container?: number;
}
// 
export interface QueryResult_SF$Common$Documents$Category {
	//
	//类型:SF.Sys.Entities.ISummary
	Summary?: SF$Sys$Entities$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Common.Documents.Category[]
	Items?: SF$Common$Documents$Category[];
}
// 
export interface SF$Sys$NetworkService$Metadata$Library extends SF$Sys$Metadata$Models$Library {
	//
	//类型:SF.Sys.NetworkService.Metadata.Service[]
	Services?: SF$Sys$NetworkService$Metadata$Service[];
}
// 
export interface SF$Sys$Metadata$Models$Library {
	//
	//类型:SF.Sys.Metadata.Models.Type[]
	Types?: SF$Sys$Metadata$Models$Type[];
}
// 
export interface SF$Sys$Metadata$Models$Type extends SF$Sys$Metadata$Models$Entity {
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
	//类型:SF.Sys.Metadata.Models.Property[]
	Properties?: SF$Sys$Metadata$Models$Property[];
}
// 
export interface SF$Sys$Metadata$Models$Entity {
	//
	//类型:SF.Sys.Metadata.Models.Attribute[]
	Attributes?: SF$Sys$Metadata$Models$Attribute[];
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
export interface SF$Sys$Metadata$Models$Attribute {
	//
	//类型:string
	Type?: string;
	//
	//类型:string
	Values?: string;
}
// 
export interface SF$Sys$Metadata$Models$Property extends SF$Sys$Metadata$Models$TypedEntity {
	//
	//类型:bool
	Optional?: boolean;
	//
	//类型:string
	DefaultValue?: string;
}
// 
export interface SF$Sys$Metadata$Models$TypedEntity extends SF$Sys$Metadata$Models$Entity {
	//
	//类型:string
	Type?: string;
}
// 
export interface SF$Sys$NetworkService$Metadata$Service extends SF$Sys$Metadata$Models$Entity {
	//
	//类型:SF.Sys.NetworkService.Metadata.Method[]
	Methods?: SF$Sys$NetworkService$Metadata$Method[];
	//
	//类型:SF.Sys.NetworkService.Metadata.GrantInfo
	GrantInfo?: SF$Sys$NetworkService$Metadata$GrantInfo;
}
// 
export interface SF$Sys$NetworkService$Metadata$Method extends SF$Sys$Metadata$Models$Method {
	//
	//类型:string
	HeavyParameter?: string;
	//
	//类型:SF.Sys.NetworkService.Metadata.GrantInfo
	GrantInfo?: SF$Sys$NetworkService$Metadata$GrantInfo;
}
// 
export interface SF$Sys$Metadata$Models$Method extends SF$Sys$Metadata$Models$TypedEntity {
	//
	//类型:SF.Sys.Metadata.Models.Parameter[]
	Parameters?: SF$Sys$Metadata$Models$Parameter[];
}
// 
export interface SF$Sys$Metadata$Models$Parameter extends SF$Sys$Metadata$Models$TypedEntity {
	//
	//类型:bool
	Optional?: boolean;
	//
	//类型:string
	DefaultValue?: string;
}
// 
export interface SF$Sys$NetworkService$Metadata$GrantInfo {
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
export interface SF$Sys$IContent {
	//
	//类型:string
	ContentType?: string;
	//
	//类型:string
	FileName?: string;
	//
	//类型:System.Text.Encoding
	Encoding?: System$Text$Encoding;
}
// 
export interface System$Text$Encoding {
	//
	//类型:string
	BodyName?: string;
	//
	//类型:string
	EncodingName?: string;
	//
	//类型:string
	HeaderName?: string;
	//
	//类型:string
	WebName?: string;
	//
	//类型:int
	WindowsCodePage: number;
	//
	//类型:bool
	IsBrowserDisplay?: boolean;
	//
	//类型:bool
	IsBrowserSave?: boolean;
	//
	//类型:bool
	IsMailNewsDisplay?: boolean;
	//
	//类型:bool
	IsMailNewsSave?: boolean;
	//
	//类型:bool
	IsSingleByte?: boolean;
	//
	//类型:System.Text.EncoderFallback
	EncoderFallback?: System$Text$EncoderFallback;
	//
	//类型:System.Text.DecoderFallback
	DecoderFallback?: System$Text$DecoderFallback;
	//
	//类型:bool
	IsReadOnly?: boolean;
	//
	//类型:int
	CodePage: number;
}
// 
export interface System$Text$EncoderFallback {
	//
	//类型:int
	MaxCharCount: number;
}
// 
export interface System$Text$DecoderFallback {
	//
	//类型:int
	MaxCharCount: number;
}
//
//
export const ServiceFeatureControl={
//
//
Init(
	//Id
	//类型:string
	Id?: string,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'ServiceFeatureControl',
		'Init',"",
		{
			Id:Id
		},
		);
},
}
//服务定义管理
//定义系统内置服务
export const ServiceDeclarationManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$Services$Management$Models$ServiceDeclaration> {
	return _invoker(
		'ServiceDeclarationManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_string[]
	Ids: ObjectKey_string[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$Services$Management$Models$ServiceDeclaration[]> {
	return _invoker(
		'ServiceDeclarationManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Sys.Services.Management.ServiceDeclarationQueryArgument
	Arg: SF$Sys$Services$Management$ServiceDeclarationQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Sys$Services$Management$Models$ServiceDeclaration> {
	return _invoker(
		'ServiceDeclarationManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Sys.Services.Management.ServiceDeclarationQueryArgument
	Arg: SF$Sys$Services$Management$ServiceDeclarationQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'ServiceDeclarationManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//服务实现管理
//系统内置服务实现
export const ServiceImplementManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$Services$Management$Models$ServiceImplement> {
	return _invoker(
		'ServiceImplementManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_string[]
	Ids: ObjectKey_string[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$Services$Management$Models$ServiceImplement[]> {
	return _invoker(
		'ServiceImplementManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Sys.Services.Management.ServiceImplementQueryArgument
	Arg: SF$Sys$Services$Management$ServiceImplementQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Sys$Services$Management$Models$ServiceImplement> {
	return _invoker(
		'ServiceImplementManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Sys.Services.Management.ServiceImplementQueryArgument
	Arg: SF$Sys$Services$Management$ServiceImplementQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'ServiceImplementManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//服务实例管理
//系统内置服务实例
export const ServiceInstanceManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ServiceInstanceManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ServiceInstanceManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$Services$Management$Models$ServiceInstanceEditable> {
	return _invoker(
		'ServiceInstanceManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Sys.Services.Management.Models.ServiceInstanceEditable
	Entity: SF$Sys$Services$Management$Models$ServiceInstanceEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'ServiceInstanceManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Sys.Services.Management.Models.ServiceInstanceEditable
	Entity: SF$Sys$Services$Management$Models$ServiceInstanceEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ServiceInstanceManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$Services$Management$Models$ServiceInstanceInternal> {
	return _invoker(
		'ServiceInstanceManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$Services$Management$Models$ServiceInstanceInternal[]> {
	return _invoker(
		'ServiceInstanceManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Sys.Services.Management.ServiceInstanceQueryArgument
	Arg: SF$Sys$Services$Management$ServiceInstanceQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Sys$Services$Management$Models$ServiceInstanceInternal> {
	return _invoker(
		'ServiceInstanceManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Sys.Services.Management.ServiceInstanceQueryArgument
	Arg: SF$Sys$Services$Management$ServiceInstanceQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'ServiceInstanceManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//媒体附件支持
//
export const Media={
//
//
Upload(
	//returnJson
	//类型:bool
	returnJson?: boolean,
	__opts?:ICallOptions
	) : PromiseLike<any> {
	return _invoker(
		'Media',
		'Upload',"",
		{
			returnJson:returnJson
		},
		);
},
//
//
Clip(
	//src
	//类型:string
	src: string,
	//x
	//类型:double
	x: number,
	//y
	//类型:double
	y: number,
	//w
	//类型:double
	w: number,
	//h
	//类型:double
	h: number,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'Media',
		'Clip',"",
		{
			src:src,
			x:x,
			y:y,
			w:w,
			h:h
		},
		);
},
//
//
Get(
	//id
	//类型:string
	id: string,
	//format
	//类型:string
	format?: string,
	__opts?:ICallOptions
	) : PromiseLike<any> {
	return _invoker(
		'Media',
		'Get',"",
		{
			id:id,
			format:format
		},
		);
},
}
//菜单管理
//
export const Menu={
//
//
GetMenu(
	//Ident
	//类型:string
	Ident: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$MenuServices$MenuItem[]> {
	return _invoker(
		'Menu',
		'GetMenu',"",
		{
			Ident:Ident
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$MenuServices$Models$Menu> {
	return _invoker(
		'Menu',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$MenuServices$Models$Menu[]> {
	return _invoker(
		'Menu',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Sys.MenuServices.MenuQueryArgument
	Arg: SF$Sys$MenuServices$MenuQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Sys$MenuServices$Models$Menu> {
	return _invoker(
		'Menu',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Sys.MenuServices.MenuQueryArgument
	Arg: SF$Sys$MenuServices$MenuQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'Menu',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'Menu',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'Menu',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$MenuServices$Models$MenuEditable> {
	return _invoker(
		'Menu',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Sys.MenuServices.Models.MenuEditable
	Entity: SF$Sys$MenuServices$Models$MenuEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'Menu',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Sys.MenuServices.Models.MenuEditable
	Entity: SF$Sys$MenuServices$Models$MenuEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'Menu',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//患者管理
//
export const PatientManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Patients$Models$Patient> {
	return _invoker(
		'PatientManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Patients$Models$Patient[]> {
	return _invoker(
		'PatientManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Patients.PatientQueryArgument
	Arg: KDL$Services$Treatments$Patients$PatientQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Patients$Models$Patient> {
	return _invoker(
		'PatientManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Patients.PatientQueryArgument
	Arg: KDL$Services$Treatments$Patients$PatientQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'PatientManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PatientManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PatientManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Patients$Models$Patient> {
	return _invoker(
		'PatientManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Patients.Models.Patient
	Entity: KDL$Services$Treatments$Patients$Models$Patient,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'PatientManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Patients.Models.Patient
	Entity: KDL$Services$Treatments$Patients$Models$Patient,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PatientManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//患者患病管理
//
export const PatientDiseaseManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Patients$Models$PatientDisease> {
	return _invoker(
		'PatientDiseaseManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Patients$Models$PatientDisease[]> {
	return _invoker(
		'PatientDiseaseManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Patients.PatientDiseaseQueryArgument
	Arg: KDL$Services$Treatments$Patients$PatientDiseaseQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Patients$Models$PatientDisease> {
	return _invoker(
		'PatientDiseaseManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Patients.PatientDiseaseQueryArgument
	Arg: KDL$Services$Treatments$Patients$PatientDiseaseQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'PatientDiseaseManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PatientDiseaseManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PatientDiseaseManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Patients$Models$PatientDisease> {
	return _invoker(
		'PatientDiseaseManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Patients.Models.PatientDisease
	Entity: KDL$Services$Treatments$Patients$Models$PatientDisease,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'PatientDiseaseManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Patients.Models.PatientDisease
	Entity: KDL$Services$Treatments$Patients$Models$PatientDisease,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PatientDiseaseManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//用药历史管理
//
export const PharmaconHistoryManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Patients$Models$PharmaconHistory> {
	return _invoker(
		'PharmaconHistoryManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Patients$Models$PharmaconHistory[]> {
	return _invoker(
		'PharmaconHistoryManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Patients.PharmaconHistoryQueryArgument
	Arg: KDL$Services$Treatments$Patients$PharmaconHistoryQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Patients$Models$PharmaconHistory> {
	return _invoker(
		'PharmaconHistoryManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Patients.PharmaconHistoryQueryArgument
	Arg: KDL$Services$Treatments$Patients$PharmaconHistoryQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'PharmaconHistoryManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconHistoryManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconHistoryManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Patients$Models$PharmaconHistory> {
	return _invoker(
		'PharmaconHistoryManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Patients.Models.PharmaconHistory
	Entity: KDL$Services$Treatments$Patients$Models$PharmaconHistory,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'PharmaconHistoryManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Patients.Models.PharmaconHistory
	Entity: KDL$Services$Treatments$Patients$Models$PharmaconHistory,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconHistoryManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//服药计划管理
//
export const MedicationPlanManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicationPlan> {
	return _invoker(
		'MedicationPlanManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicationPlan[]> {
	return _invoker(
		'MedicationPlanManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Medications.MedicationPlanQueryArgument
	Arg: KDL$Services$Treatments$Medications$MedicationPlanQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Medications$Models$MedicationPlan> {
	return _invoker(
		'MedicationPlanManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Medications.MedicationPlanQueryArgument
	Arg: KDL$Services$Treatments$Medications$MedicationPlanQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'MedicationPlanManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicationPlanManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicationPlanManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicationPlan> {
	return _invoker(
		'MedicationPlanManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Medications.Models.MedicationPlan
	Entity: KDL$Services$Treatments$Medications$Models$MedicationPlan,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'MedicationPlanManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Medications.Models.MedicationPlan
	Entity: KDL$Services$Treatments$Medications$Models$MedicationPlan,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicationPlanManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//服药记录管理
//
export const MedicationRecordManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicationRecord> {
	return _invoker(
		'MedicationRecordManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicationRecord[]> {
	return _invoker(
		'MedicationRecordManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Medications.MedicationRecordQueryArgument
	Arg: KDL$Services$Treatments$Medications$MedicationRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Medications$Models$MedicationRecord> {
	return _invoker(
		'MedicationRecordManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Medications.MedicationRecordQueryArgument
	Arg: KDL$Services$Treatments$Medications$MedicationRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'MedicationRecordManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicationRecordManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicationRecordManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicationRecord> {
	return _invoker(
		'MedicationRecordManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Medications.Models.MedicationRecord
	Entity: KDL$Services$Treatments$Medications$Models$MedicationRecord,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'MedicationRecordManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Medications.Models.MedicationRecord
	Entity: KDL$Services$Treatments$Medications$Models$MedicationRecord,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicationRecordManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//药槽设置管理
//
export const MedicineBoxCellSettingManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicineBoxCellSetting> {
	return _invoker(
		'MedicineBoxCellSettingManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicineBoxCellSetting[]> {
	return _invoker(
		'MedicineBoxCellSettingManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Medications.MedicineBoxCellSettingQueryArgument
	Arg: KDL$Services$Treatments$Medications$MedicineBoxCellSettingQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Medications$Models$MedicineBoxCellSetting> {
	return _invoker(
		'MedicineBoxCellSettingManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Medications.MedicineBoxCellSettingQueryArgument
	Arg: KDL$Services$Treatments$Medications$MedicineBoxCellSettingQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'MedicineBoxCellSettingManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicineBoxCellSettingManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicineBoxCellSettingManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicineBoxCellSetting> {
	return _invoker(
		'MedicineBoxCellSettingManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Medications.Models.MedicineBoxCellSetting
	Entity: KDL$Services$Treatments$Medications$Models$MedicineBoxCellSetting,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'MedicineBoxCellSettingManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Medications.Models.MedicineBoxCellSetting
	Entity: KDL$Services$Treatments$Medications$Models$MedicineBoxCellSetting,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicineBoxCellSettingManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//装药计划管理
//
export const MedicineBoxReloadPlanManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicineBoxReloadPlan> {
	return _invoker(
		'MedicineBoxReloadPlanManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicineBoxReloadPlan[]> {
	return _invoker(
		'MedicineBoxReloadPlanManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Medications.MedicineBoxReloadPlanQueryArgument
	Arg: KDL$Services$Treatments$Medications$MedicineBoxReloadPlanQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Medications$Models$MedicineBoxReloadPlan> {
	return _invoker(
		'MedicineBoxReloadPlanManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Medications.MedicineBoxReloadPlanQueryArgument
	Arg: KDL$Services$Treatments$Medications$MedicineBoxReloadPlanQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'MedicineBoxReloadPlanManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicineBoxReloadPlanManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicineBoxReloadPlanManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Medications$Models$MedicineBoxReloadPlan> {
	return _invoker(
		'MedicineBoxReloadPlanManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Medications.Models.MedicineBoxReloadPlan
	Entity: KDL$Services$Treatments$Medications$Models$MedicineBoxReloadPlan,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'MedicineBoxReloadPlanManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Medications.Models.MedicineBoxReloadPlan
	Entity: KDL$Services$Treatments$Medications$Models$MedicineBoxReloadPlan,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MedicineBoxReloadPlanManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//诊断记录管理
//
export const DiagnosticRecordManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecord> {
	return _invoker(
		'DiagnosticRecordManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecord[]> {
	return _invoker(
		'DiagnosticRecordManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Diagnostics.DiagnosticRecordQueryArgument
	Arg: KDL$Services$Treatments$Diagnostics$DiagnosticRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecord> {
	return _invoker(
		'DiagnosticRecordManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Diagnostics.DiagnosticRecordQueryArgument
	Arg: KDL$Services$Treatments$Diagnostics$DiagnosticRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DiagnosticRecordManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DiagnosticRecordManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DiagnosticRecordManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecord> {
	return _invoker(
		'DiagnosticRecordManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Diagnostics.Models.DiagnosticRecord
	Entity: KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecord,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DiagnosticRecordManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Diagnostics.Models.DiagnosticRecord
	Entity: KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecord,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DiagnosticRecordManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//诊断记录项目
//
export const DiagnosticRecordItemManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecordItem> {
	return _invoker(
		'DiagnosticRecordItemManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecordItem[]> {
	return _invoker(
		'DiagnosticRecordItemManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Diagnostics.DiagnosticRecordItemQueryArgument
	Arg: KDL$Services$Treatments$Diagnostics$DiagnosticRecordItemQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecordItem> {
	return _invoker(
		'DiagnosticRecordItemManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Diagnostics.DiagnosticRecordItemQueryArgument
	Arg: KDL$Services$Treatments$Diagnostics$DiagnosticRecordItemQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DiagnosticRecordItemManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DiagnosticRecordItemManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DiagnosticRecordItemManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecordItem> {
	return _invoker(
		'DiagnosticRecordItemManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Diagnostics.Models.DiagnosticRecordItem
	Entity: KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecordItem,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DiagnosticRecordItemManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Diagnostics.Models.DiagnosticRecordItem
	Entity: KDL$Services$Treatments$Diagnostics$Models$DiagnosticRecordItem,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DiagnosticRecordItemManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//提醒计划管理
//
export const RemindPlanManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Reminds$Models$RemindPlan> {
	return _invoker(
		'RemindPlanManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Reminds$Models$RemindPlan[]> {
	return _invoker(
		'RemindPlanManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Reminds.RemindPlanQueryArgument
	Arg: KDL$Services$Treatments$Reminds$RemindPlanQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Reminds$Models$RemindPlan> {
	return _invoker(
		'RemindPlanManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Reminds.RemindPlanQueryArgument
	Arg: KDL$Services$Treatments$Reminds$RemindPlanQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'RemindPlanManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RemindPlanManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RemindPlanManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Reminds$Models$RemindPlan> {
	return _invoker(
		'RemindPlanManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Reminds.Models.RemindPlan
	Entity: KDL$Services$Treatments$Reminds$Models$RemindPlan,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'RemindPlanManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Reminds.Models.RemindPlan
	Entity: KDL$Services$Treatments$Reminds$Models$RemindPlan,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RemindPlanManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//提醒动作管理
//
export const RemindActionManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Reminds$Models$RemindAction> {
	return _invoker(
		'RemindActionManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Reminds$Models$RemindAction[]> {
	return _invoker(
		'RemindActionManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Reminds.RemindActionQueryArgument
	Arg: KDL$Services$Treatments$Reminds$RemindActionQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Reminds$Models$RemindAction> {
	return _invoker(
		'RemindActionManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Reminds.RemindActionQueryArgument
	Arg: KDL$Services$Treatments$Reminds$RemindActionQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'RemindActionManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RemindActionManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RemindActionManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Reminds$Models$RemindAction> {
	return _invoker(
		'RemindActionManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Reminds.Models.RemindAction
	Entity: KDL$Services$Treatments$Reminds$Models$RemindAction,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'RemindActionManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Reminds.Models.RemindAction
	Entity: KDL$Services$Treatments$Reminds$Models$RemindAction,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RemindActionManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//提醒记录管理
//
export const RemindRecordManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Reminds$Models$RemindRecord> {
	return _invoker(
		'RemindRecordManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Reminds$Models$RemindRecord[]> {
	return _invoker(
		'RemindRecordManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Treatments.Reminds.RemindRecordQueryArgument
	Arg: KDL$Services$Treatments$Reminds$RemindRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Treatments$Reminds$Models$RemindRecord> {
	return _invoker(
		'RemindRecordManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Treatments.Reminds.RemindRecordQueryArgument
	Arg: KDL$Services$Treatments$Reminds$RemindRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'RemindRecordManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RemindRecordManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RemindRecordManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Treatments$Reminds$Models$RemindRecord> {
	return _invoker(
		'RemindRecordManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Treatments.Reminds.Models.RemindRecord
	Entity: KDL$Services$Treatments$Reminds$Models$RemindRecord,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'RemindRecordManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Treatments.Reminds.Models.RemindRecord
	Entity: KDL$Services$Treatments$Reminds$Models$RemindRecord,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RemindRecordManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//设备管理
//
export const DeviceManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$Device> {
	return _invoker(
		'DeviceManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$Device[]> {
	return _invoker(
		'DeviceManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Devices.DeviceQueryArgument
	Arg: KDL$Services$Devices$DeviceQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Devices$Models$Device> {
	return _invoker(
		'DeviceManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Devices.DeviceQueryArgument
	Arg: KDL$Services$Devices$DeviceQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DeviceManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$Device> {
	return _invoker(
		'DeviceManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Devices.Models.Device
	Entity: KDL$Services$Devices$Models$Device,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DeviceManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Devices.Models.Device
	Entity: KDL$Services$Devices$Models$Device,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//设备型号管理
//
export const DeviceModelManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$DeviceModel> {
	return _invoker(
		'DeviceModelManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$DeviceModel[]> {
	return _invoker(
		'DeviceModelManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Devices.DeviceModelQueryArgument
	Arg: KDL$Services$Devices$DeviceModelQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Devices$Models$DeviceModel> {
	return _invoker(
		'DeviceModelManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Devices.DeviceModelQueryArgument
	Arg: KDL$Services$Devices$DeviceModelQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DeviceModelManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceModelManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceModelManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$DeviceModel> {
	return _invoker(
		'DeviceModelManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Devices.Models.DeviceModel
	Entity: KDL$Services$Devices$Models$DeviceModel,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DeviceModelManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Devices.Models.DeviceModel
	Entity: KDL$Services$Devices$Models$DeviceModel,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceModelManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//设备产品管理
//
export const DeviceProductManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$DeviceProduct> {
	return _invoker(
		'DeviceProductManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$DeviceProduct[]> {
	return _invoker(
		'DeviceProductManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Devices.DeviceProductQueryArgument
	Arg: KDL$Services$Devices$DeviceProductQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Devices$Models$DeviceProduct> {
	return _invoker(
		'DeviceProductManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Devices.DeviceProductQueryArgument
	Arg: KDL$Services$Devices$DeviceProductQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DeviceProductManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceProductManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceProductManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$DeviceProduct> {
	return _invoker(
		'DeviceProductManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Devices.Models.DeviceProduct
	Entity: KDL$Services$Devices$Models$DeviceProduct,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DeviceProductManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Devices.Models.DeviceProduct
	Entity: KDL$Services$Devices$Models$DeviceProduct,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceProductManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//设备批次管理
//
export const DeviceBatchManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$DeviceBatch> {
	return _invoker(
		'DeviceBatchManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$DeviceBatch[]> {
	return _invoker(
		'DeviceBatchManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Devices.DeviceBatchQueryArgument
	Arg: KDL$Services$Devices$DeviceBatchQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Devices$Models$DeviceBatch> {
	return _invoker(
		'DeviceBatchManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Devices.DeviceBatchQueryArgument
	Arg: KDL$Services$Devices$DeviceBatchQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DeviceBatchManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceBatchManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceBatchManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Devices$Models$DeviceBatch> {
	return _invoker(
		'DeviceBatchManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Devices.Models.DeviceBatch
	Entity: KDL$Services$Devices$Models$DeviceBatch,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DeviceBatchManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Devices.Models.DeviceBatch
	Entity: KDL$Services$Devices$Models$DeviceBatch,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DeviceBatchManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//
//
export const DoctorManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Doctors$Models$Doctor> {
	return _invoker(
		'DoctorManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Doctors$Models$Doctor[]> {
	return _invoker(
		'DoctorManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Doctors.DoctorQueryArgument
	Arg: KDL$Services$Doctors$DoctorQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Doctors$Models$Doctor> {
	return _invoker(
		'DoctorManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Doctors.DoctorQueryArgument
	Arg: KDL$Services$Doctors$DoctorQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DoctorManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DoctorManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DoctorManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Doctors$Models$Doctor> {
	return _invoker(
		'DoctorManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Doctors.Models.Doctor
	Entity: KDL$Services$Doctors$Models$Doctor,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DoctorManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Doctors.Models.Doctor
	Entity: KDL$Services$Doctors$Models$Doctor,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DoctorManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//药品管理
//
export const PharmaconManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$Pharmacon> {
	return _invoker(
		'PharmaconManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$Pharmacon[]> {
	return _invoker(
		'PharmaconManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Pharmacons.PharmaconQueryArgument
	Arg: KDL$Services$Pharmacons$PharmaconQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Pharmacons$Models$Pharmacon> {
	return _invoker(
		'PharmaconManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Pharmacons.PharmaconQueryArgument
	Arg: KDL$Services$Pharmacons$PharmaconQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'PharmaconManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$Pharmacon> {
	return _invoker(
		'PharmaconManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Pharmacons.Models.Pharmacon
	Entity: KDL$Services$Pharmacons$Models$Pharmacon,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'PharmaconManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Pharmacons.Models.Pharmacon
	Entity: KDL$Services$Pharmacons$Models$Pharmacon,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//药品规格管理
//
export const PharmaconSpecManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$PharmaconSpec> {
	return _invoker(
		'PharmaconSpecManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$PharmaconSpec[]> {
	return _invoker(
		'PharmaconSpecManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Pharmacons.PharmaconSpecQueryArgument
	Arg: KDL$Services$Pharmacons$PharmaconSpecQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Pharmacons$Models$PharmaconSpec> {
	return _invoker(
		'PharmaconSpecManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Pharmacons.PharmaconSpecQueryArgument
	Arg: KDL$Services$Pharmacons$PharmaconSpecQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'PharmaconSpecManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconSpecManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconSpecManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$PharmaconSpec> {
	return _invoker(
		'PharmaconSpecManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Pharmacons.Models.PharmaconSpec
	Entity: KDL$Services$Pharmacons$Models$PharmaconSpec,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'PharmaconSpecManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Pharmacons.Models.PharmaconSpec
	Entity: KDL$Services$Pharmacons$Models$PharmaconSpec,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconSpecManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//药品分类管理
//
export const PharmaconCategoryManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$PharmaconCategory> {
	return _invoker(
		'PharmaconCategoryManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$PharmaconCategory[]> {
	return _invoker(
		'PharmaconCategoryManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Pharmacons.PharmaconCategoryQueryArgument
	Arg: KDL$Services$Pharmacons$PharmaconCategoryQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Pharmacons$Models$PharmaconCategory> {
	return _invoker(
		'PharmaconCategoryManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Pharmacons.PharmaconCategoryQueryArgument
	Arg: KDL$Services$Pharmacons$PharmaconCategoryQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'PharmaconCategoryManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconCategoryManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconCategoryManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$PharmaconCategory> {
	return _invoker(
		'PharmaconCategoryManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Pharmacons.Models.PharmaconCategory
	Entity: KDL$Services$Pharmacons$Models$PharmaconCategory,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'PharmaconCategoryManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Pharmacons.Models.PharmaconCategory
	Entity: KDL$Services$Pharmacons$Models$PharmaconCategory,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconCategoryManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//药品计量管理
//
export const PharmaconUnitManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$PharmaconUnit> {
	return _invoker(
		'PharmaconUnitManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$PharmaconUnit[]> {
	return _invoker(
		'PharmaconUnitManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Pharmacons.PharmaconUnitQueryArgument
	Arg: KDL$Services$Pharmacons$PharmaconUnitQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Pharmacons$Models$PharmaconUnit> {
	return _invoker(
		'PharmaconUnitManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Pharmacons.PharmaconUnitQueryArgument
	Arg: KDL$Services$Pharmacons$PharmaconUnitQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'PharmaconUnitManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconUnitManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconUnitManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Pharmacons$Models$PharmaconUnit> {
	return _invoker(
		'PharmaconUnitManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Pharmacons.Models.PharmaconUnit
	Entity: KDL$Services$Pharmacons$Models$PharmaconUnit,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'PharmaconUnitManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Pharmacons.Models.PharmaconUnit
	Entity: KDL$Services$Pharmacons$Models$PharmaconUnit,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'PharmaconUnitManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//疾病分类管理
//
export const DiseaseManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Diseases$Models$Disease> {
	return _invoker(
		'DiseaseManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Diseases$Models$Disease[]> {
	return _invoker(
		'DiseaseManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:KDL.Services.Diseases.DiseaseQueryArgument
	Arg: KDL$Services$Diseases$DiseaseQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_KDL$Services$Diseases$Models$Disease> {
	return _invoker(
		'DiseaseManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:KDL.Services.Diseases.DiseaseQueryArgument
	Arg: KDL$Services$Diseases$DiseaseQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DiseaseManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DiseaseManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DiseaseManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<KDL$Services$Diseases$Models$Disease> {
	return _invoker(
		'DiseaseManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:KDL.Services.Diseases.Models.Disease
	Entity: KDL$Services$Diseases$Models$Disease,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DiseaseManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:KDL.Services.Diseases.Models.Disease
	Entity: KDL$Services$Diseases$Models$Disease,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DiseaseManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//申明类型管理
//
export const ClaimTypeManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ClaimTypeManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ClaimTypeManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ClaimType> {
	return _invoker(
		'ClaimTypeManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.ClaimType
	Entity: SF$Auth$IdentityServices$Models$ClaimType,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_string> {
	return _invoker(
		'ClaimTypeManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.ClaimType
	Entity: SF$Auth$IdentityServices$Models$ClaimType,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ClaimTypeManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ClaimType> {
	return _invoker(
		'ClaimTypeManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_string[]
	Ids: ObjectKey_string[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ClaimType[]> {
	return _invoker(
		'ClaimTypeManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.ClaimTypeQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$ClaimTypeQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Auth$IdentityServices$Models$ClaimType> {
	return _invoker(
		'ClaimTypeManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.ClaimTypeQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$ClaimTypeQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'ClaimTypeManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//客户端配置管理
//
export const ClientConfigManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ClientConfigManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ClientConfigManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ClientConfigEditable> {
	return _invoker(
		'ClientConfigManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.ClientConfigEditable
	Entity: SF$Auth$IdentityServices$Models$ClientConfigEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'ClientConfigManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.ClientConfigEditable
	Entity: SF$Auth$IdentityServices$Models$ClientConfigEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ClientConfigManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ClientConfigInternal> {
	return _invoker(
		'ClientConfigManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ClientConfigInternal[]> {
	return _invoker(
		'ClientConfigManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.ClientConfigQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$ClientConfigQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Auth$IdentityServices$Models$ClientConfigInternal> {
	return _invoker(
		'ClientConfigManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.ClientConfigQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$ClientConfigQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'ClientConfigManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//客户端管理
//
export const ClientManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ClientManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ClientManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ClientEditable> {
	return _invoker(
		'ClientManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.ClientEditable
	Entity: SF$Auth$IdentityServices$Models$ClientEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_string> {
	return _invoker(
		'ClientManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.ClientEditable
	Entity: SF$Auth$IdentityServices$Models$ClientEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ClientManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ClientInternal> {
	return _invoker(
		'ClientManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_string[]
	Ids: ObjectKey_string[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ClientInternal[]> {
	return _invoker(
		'ClientManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.ClientQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$ClientQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Auth$IdentityServices$Models$ClientInternal> {
	return _invoker(
		'ClientManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.ClientQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$ClientQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'ClientManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//操作管理
//
export const OperationManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'OperationManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'OperationManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$OperationInternal> {
	return _invoker(
		'OperationManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.OperationInternal
	Entity: SF$Auth$IdentityServices$Models$OperationInternal,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_string> {
	return _invoker(
		'OperationManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.OperationInternal
	Entity: SF$Auth$IdentityServices$Models$OperationInternal,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'OperationManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$OperationInternal> {
	return _invoker(
		'OperationManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_string[]
	Ids: ObjectKey_string[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$OperationInternal[]> {
	return _invoker(
		'OperationManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.OperationQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$OperationQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Auth$IdentityServices$Models$OperationInternal> {
	return _invoker(
		'OperationManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.OperationQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$OperationQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'OperationManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//资源管理
//
export const ResourceManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ResourceManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ResourceManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ResourceEditable> {
	return _invoker(
		'ResourceManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.ResourceEditable
	Entity: SF$Auth$IdentityServices$Models$ResourceEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_string> {
	return _invoker(
		'ResourceManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.ResourceEditable
	Entity: SF$Auth$IdentityServices$Models$ResourceEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ResourceManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ResourceInternal> {
	return _invoker(
		'ResourceManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_string[]
	Ids: ObjectKey_string[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ResourceInternal[]> {
	return _invoker(
		'ResourceManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.ResourceQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$ResourceQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Auth$IdentityServices$Models$ResourceInternal> {
	return _invoker(
		'ResourceManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.ResourceQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$ResourceQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'ResourceManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//角色管理
//
export const RoleManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RoleManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RoleManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$RoleEditable> {
	return _invoker(
		'RoleManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.RoleEditable
	Entity: SF$Auth$IdentityServices$Models$RoleEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_string> {
	return _invoker(
		'RoleManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.RoleEditable
	Entity: SF$Auth$IdentityServices$Models$RoleEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'RoleManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$Role> {
	return _invoker(
		'RoleManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_string[]
	Ids: ObjectKey_string[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$Role[]> {
	return _invoker(
		'RoleManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.RoleQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$RoleQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Auth$IdentityServices$Models$Role> {
	return _invoker(
		'RoleManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.RoleQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$RoleQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'RoleManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//用户管理
//
export const UserManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'UserManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'UserManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$UserEditable> {
	return _invoker(
		'UserManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.UserEditable
	Entity: SF$Auth$IdentityServices$Models$UserEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'UserManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.UserEditable
	Entity: SF$Auth$IdentityServices$Models$UserEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'UserManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$UserInternal> {
	return _invoker(
		'UserManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$UserInternal[]> {
	return _invoker(
		'UserManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.UserQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$UserQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Auth$IdentityServices$Models$UserInternal> {
	return _invoker(
		'UserManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.UserQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$UserQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'UserManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Load(
	//Id
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Internals$UserData> {
	return _invoker(
		'UserManager',
		'Load',"",
		{
			Id:Id
		},
		);
},
//
//
UpdateDescription(
	//Identity
	//类型:SF.Sys.Auth.User
	Identity: SF$Sys$Auth$User,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'UserManager',
		'UpdateDescription',"Identity",
		{
			Identity:Identity
		},
		);
},
//
//
UpdateSecurity(
	//Id
	//类型:long
	Id: number,
	//PasswordHash
	//类型:string
	PasswordHash: string,
	//SecurityStamp
	//类型:byte[]
	SecurityStamp: number[],
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'UserManager',
		'UpdateSecurity',"SecurityStamp",
		{
			Id:Id,
			PasswordHash:PasswordHash,
			SecurityStamp:SecurityStamp
		},
		);
},
}
//授权范围
//
export const ScopeManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ScopeManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ScopeManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ScopeEditable> {
	return _invoker(
		'ScopeManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.ScopeEditable
	Entity: SF$Auth$IdentityServices$Models$ScopeEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_string> {
	return _invoker(
		'ScopeManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Auth.IdentityServices.Models.ScopeEditable
	Entity: SF$Auth$IdentityServices$Models$ScopeEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ScopeManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ScopeInternal> {
	return _invoker(
		'ScopeManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_string[]
	Ids: ObjectKey_string[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Auth$IdentityServices$Models$ScopeInternal[]> {
	return _invoker(
		'ScopeManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.ScopeQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$ScopeQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Auth$IdentityServices$Models$ScopeInternal> {
	return _invoker(
		'ScopeManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Auth.IdentityServices.Managers.ScopeQueryArgument
	Arg: SF$Auth$IdentityServices$Managers$ScopeQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'ScopeManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//用户服务
//
export const User={
//获取当前用户ID
//
GetCurUserId(
	__opts?:ICallOptions
	) : PromiseLike<number> {
	return _invoker(
		'User',
		'GetCurUserId',"",
null,		);
},
//获取当前用户
//
GetCurUser(
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$Auth$User> {
	return _invoker(
		'User',
		'GetCurUser',"",
null,		);
},
//登录
//
Signin(
	//登录参数
	//类型:SF.Auth.IdentityServices.SigninArgument
	Arg: SF$Auth$IdentityServices$SigninArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'User',
		'Signin',"Arg",
		{
			Arg:Arg
		},
		);
},
//注销
//
Signout(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'User',
		'Signout',"",
null,		);
},
//发送忘记密码验证消息
//
SendPasswordRecorveryCode(
	//找回密码参数
	//类型:SF.Auth.IdentityServices.SendPasswordRecorveryCodeArgument
	Arg: SF$Auth$IdentityServices$SendPasswordRecorveryCodeArgument,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'User',
		'SendPasswordRecorveryCode',"Arg",
		{
			Arg:Arg
		},
		);
},
//使用验证消息重置密码
//
ResetPasswordByRecoveryCode(
	//
	//类型:SF.Auth.IdentityServices.ResetPasswordByRecorveryCodeArgument
	Arg: SF$Auth$IdentityServices$ResetPasswordByRecorveryCodeArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'User',
		'ResetPasswordByRecoveryCode',"Arg",
		{
			Arg:Arg
		},
		);
},
//设置密码
//
SetPassword(
	//重置密码参数
	//类型:SF.Auth.IdentityServices.SetPasswordArgument
	Arg: SF$Auth$IdentityServices$SetPasswordArgument,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'User',
		'SetPassword',"Arg",
		{
			Arg:Arg
		},
		);
},
//修改用户信息
//
Update(
	//用户信息实体
	//类型:SF.Sys.Auth.User
	User: SF$Sys$Auth$User,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'User',
		'Update',"User",
		{
			User:User
		},
		);
},
//从访问令牌提取身份ID
//
ValidateAccessToken(
	//访问令牌
	//类型:string
	AccessToken: string,
	__opts?:ICallOptions
	) : PromiseLike<number> {
	return _invoker(
		'User',
		'ValidateAccessToken',"",
		{
			AccessToken:AccessToken
		},
		);
},
//发送用户创建验证信息
//
SendCreateIdentityVerifyCode(
	//用户创建参数
	//类型:SF.Auth.IdentityServices.SendCreateIdentityVerifyCodeArgument
	Arg: SF$Auth$IdentityServices$SendCreateIdentityVerifyCodeArgument,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'User',
		'SendCreateIdentityVerifyCode',"Arg",
		{
			Arg:Arg
		},
		);
},
//注册用户
//
Signup(
	//注册参数
	//类型:SF.Auth.IdentityServices.SignupArgument
	Arg: SF$Auth$IdentityServices$SignupArgument,
	//是否验证验证信息
	//类型:bool
	VerifyCode: boolean,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'User',
		'Signup',"Arg",
		{
			Arg:Arg,
			VerifyCode:VerifyCode
		},
		);
},
//根据用户ID获取身份信息
//
GetUser(
	//用户ID
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Sys$Auth$User> {
	return _invoker(
		'User',
		'GetUser',"",
		{
			Id:Id
		},
		);
},
}
//文本消息记录
//
export const MsgRecordManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$TextMessages$Models$MsgRecord> {
	return _invoker(
		'MsgRecordManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$TextMessages$Models$MsgRecord[]> {
	return _invoker(
		'MsgRecordManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Common.TextMessages.Management.MsgRecordQueryArgument
	Arg: SF$Common$TextMessages$Management$MsgRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$TextMessages$Models$MsgRecord> {
	return _invoker(
		'MsgRecordManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Common.TextMessages.Management.MsgRecordQueryArgument
	Arg: SF$Common$TextMessages$Management$MsgRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'MsgRecordManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//文本消息记录
//
export const MsgActionRecordManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$TextMessages$Models$MsgActionRecord> {
	return _invoker(
		'MsgActionRecordManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$TextMessages$Models$MsgActionRecord[]> {
	return _invoker(
		'MsgActionRecordManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Common.TextMessages.Management.MsgActionRecordQueryArgument
	Arg: SF$Common$TextMessages$Management$MsgActionRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$TextMessages$Models$MsgActionRecord> {
	return _invoker(
		'MsgActionRecordManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Common.TextMessages.Management.MsgActionRecordQueryArgument
	Arg: SF$Common$TextMessages$Management$MsgActionRecordQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'MsgActionRecordManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
}
//文本消息记录
//
export const MsgPolicyManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$TextMessages$Models$MsgPolicy> {
	return _invoker(
		'MsgPolicyManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$TextMessages$Models$MsgPolicy[]> {
	return _invoker(
		'MsgPolicyManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Common.TextMessages.Management.MsgPolicyQueryArgument
	Arg: SF$Common$TextMessages$Management$MsgPolicyQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$TextMessages$Models$MsgPolicy> {
	return _invoker(
		'MsgPolicyManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Common.TextMessages.Management.MsgPolicyQueryArgument
	Arg: SF$Common$TextMessages$Management$MsgPolicyQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'MsgPolicyManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MsgPolicyManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MsgPolicyManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$TextMessages$Models$MsgPolicy> {
	return _invoker(
		'MsgPolicyManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Common.TextMessages.Models.MsgPolicy
	Entity: SF$Common$TextMessages$Models$MsgPolicy,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'MsgPolicyManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Common.TextMessages.Models.MsgPolicy
	Entity: SF$Common$TextMessages$Models$MsgPolicy,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MsgPolicyManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//会员
//
export const MemberManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Members$Models$MemberInternal> {
	return _invoker(
		'MemberManager',
		'Get',"",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:long[]
	Ids: number[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Members$Models$MemberInternal[]> {
	return _invoker(
		'MemberManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Common.Members.MemberQueryArgument
	Arg: SF$Common$Members$MemberQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Members$Models$MemberInternal> {
	return _invoker(
		'MemberManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Common.Members.MemberQueryArgument
	Arg: SF$Common$Members$MemberQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_long> {
	return _invoker(
		'MemberManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:long
	Key: number,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MemberManager',
		'Remove',"",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MemberManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:long
	Key: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Members$Models$MemberEditable> {
	return _invoker(
		'MemberManager',
		'LoadForEdit',"",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Common.Members.Models.MemberEditable
	Entity: SF$Common$Members$Models$MemberEditable,
	__opts?:ICallOptions
	) : PromiseLike<number> {
	return _invoker(
		'MemberManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Common.Members.Models.MemberEditable
	Entity: SF$Common$Members$Models$MemberEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'MemberManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//系统管理员
//
export const AdminManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Admins$Models$AdminInternal> {
	return _invoker(
		'AdminManager',
		'Get',"",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:long[]
	Ids: number[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Admins$Models$AdminInternal[]> {
	return _invoker(
		'AdminManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Common.Admins.AdminQueryArgument
	Arg: SF$Common$Admins$AdminQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Admins$Models$AdminInternal> {
	return _invoker(
		'AdminManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Common.Admins.AdminQueryArgument
	Arg: SF$Common$Admins$AdminQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_long> {
	return _invoker(
		'AdminManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:long
	Key: number,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'AdminManager',
		'Remove',"",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'AdminManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:long
	Key: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Admins$Models$AdminEditable> {
	return _invoker(
		'AdminManager',
		'LoadForEdit',"",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Common.Admins.Models.AdminEditable
	Entity: SF$Common$Admins$Models$AdminEditable,
	__opts?:ICallOptions
	) : PromiseLike<number> {
	return _invoker(
		'AdminManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Common.Admins.Models.AdminEditable
	Entity: SF$Common$Admins$Models$AdminEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'AdminManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//前端站点管理
//
export const SiteManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_string
	Key: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$FrontEndContents$Site> {
	return _invoker(
		'SiteManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Common.FrontEndContents.Site
	Entity: SF$Common$FrontEndContents$Site,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_string> {
	return _invoker(
		'SiteManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Common.FrontEndContents.Site
	Entity: SF$Common$FrontEndContents$Site,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_string
	Id: ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$FrontEndContents$Site> {
	return _invoker(
		'SiteManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_string[]
	Ids: ObjectKey_string[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$FrontEndContents$Site[]> {
	return _invoker(
		'SiteManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:QueryArgument_ObjectKey_string
	Arg: QueryArgument_ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$FrontEndContents$Site> {
	return _invoker(
		'SiteManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:QueryArgument_ObjectKey_string
	Arg: QueryArgument_ObjectKey_string,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_string> {
	return _invoker(
		'SiteManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
FindTemplateId(
	//site
	//类型:string
	site: string,
	__opts?:ICallOptions
	) : PromiseLike<number> {
	return _invoker(
		'SiteManager',
		'FindTemplateId',"",
		{
			site:site
		},
		);
},
}
//前端站点模板管理
//
export const SiteTemplateManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteTemplateManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteTemplateManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$FrontEndContents$SiteTemplate> {
	return _invoker(
		'SiteTemplateManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Common.FrontEndContents.SiteTemplate
	Entity: SF$Common$FrontEndContents$SiteTemplate,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'SiteTemplateManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Common.FrontEndContents.SiteTemplate
	Entity: SF$Common$FrontEndContents$SiteTemplate,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'SiteTemplateManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$FrontEndContents$SiteTemplate> {
	return _invoker(
		'SiteTemplateManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$FrontEndContents$SiteTemplate[]> {
	return _invoker(
		'SiteTemplateManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Common.FrontEndContents.SiteTemplateQueryArgument
	Arg: SF$Common$FrontEndContents$SiteTemplateQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$FrontEndContents$SiteTemplate> {
	return _invoker(
		'SiteTemplateManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Common.FrontEndContents.SiteTemplateQueryArgument
	Arg: SF$Common$FrontEndContents$SiteTemplateQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'SiteTemplateManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
LoadConfig(
	//templateId
	//类型:long
	templateId: number,
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'SiteTemplateManager',
		'LoadConfig',"",
		{
			templateId:templateId
		},
		);
},
}
//前端内容管理
//
export const ContentManager={
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ContentManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ContentManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$FrontEndContents$Content> {
	return _invoker(
		'ContentManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Common.FrontEndContents.Content
	Entity: SF$Common$FrontEndContents$Content,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'ContentManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Common.FrontEndContents.Content
	Entity: SF$Common$FrontEndContents$Content,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ContentManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$FrontEndContents$Content> {
	return _invoker(
		'ContentManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$FrontEndContents$Content[]> {
	return _invoker(
		'ContentManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Common.FrontEndContents.ContentQueryArgument
	Arg: SF$Common$FrontEndContents$ContentQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$FrontEndContents$Content> {
	return _invoker(
		'ContentManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Common.FrontEndContents.ContentQueryArgument
	Arg: SF$Common$FrontEndContents$ContentQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'ContentManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
LoadContent(
	//contentId
	//类型:long
	contentId: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$FrontEndContents$IContent> {
	return _invoker(
		'ContentManager',
		'LoadContent',"",
		{
			contentId:contentId
		},
		);
},
}
//分类管理
//
export const DocumentCategoryManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Management$CategoryInternal> {
	return _invoker(
		'DocumentCategoryManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Management$CategoryInternal[]> {
	return _invoker(
		'DocumentCategoryManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Common.Documents.Management.DocumentCategoryQueryArgument
	Arg: SF$Common$Documents$Management$DocumentCategoryQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Documents$Management$CategoryInternal> {
	return _invoker(
		'DocumentCategoryManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Common.Documents.Management.DocumentCategoryQueryArgument
	Arg: SF$Common$Documents$Management$DocumentCategoryQueryArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DocumentCategoryManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentCategoryManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentCategoryManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Management$CategoryInternal> {
	return _invoker(
		'DocumentCategoryManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Common.Documents.Management.CategoryInternal
	Entity: SF$Common$Documents$Management$CategoryInternal,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DocumentCategoryManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Common.Documents.Management.CategoryInternal
	Entity: SF$Common$Documents$Management$CategoryInternal,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentCategoryManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//文档管理
//
export const DocumentManager={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Management$DocumentInternal> {
	return _invoker(
		'DocumentManager',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//
//
BatchGet(
	//Ids
	//类型:ObjectKey_long[]
	Ids: ObjectKey_long[],
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Management$DocumentInternal[]> {
	return _invoker(
		'DocumentManager',
		'BatchGet',"Ids",
		{
			Ids:Ids
		},
		);
},
//
//
Query(
	//Arg
	//类型:SF.Common.Documents.Management.DocumentQueryArguments
	Arg: SF$Common$Documents$Management$DocumentQueryArguments,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Documents$Management$DocumentInternal> {
	return _invoker(
		'DocumentManager',
		'Query',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
QueryIdents(
	//Arg
	//类型:SF.Common.Documents.Management.DocumentQueryArguments
	Arg: SF$Common$Documents$Management$DocumentQueryArguments,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_ObjectKey_long> {
	return _invoker(
		'DocumentManager',
		'QueryIdents',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
Remove(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentManager',
		'Remove',"Key",
		{
			Key:Key
		},
		);
},
//
//
RemoveAll(
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentManager',
		'RemoveAll',"",
null,		);
},
//
//
LoadForEdit(
	//Key
	//类型:ObjectKey_long
	Key: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Management$DocumentEditable> {
	return _invoker(
		'DocumentManager',
		'LoadForEdit',"Key",
		{
			Key:Key
		},
		);
},
//
//
Create(
	//Entity
	//类型:SF.Common.Documents.Management.DocumentEditable
	Entity: SF$Common$Documents$Management$DocumentEditable,
	__opts?:ICallOptions
	) : PromiseLike<ObjectKey_long> {
	return _invoker(
		'DocumentManager',
		'Create',"Entity",
		{
			Entity:Entity
		},
		);
},
//
//
Update(
	//Entity
	//类型:SF.Common.Documents.Management.DocumentEditable
	Entity: SF$Common$Documents$Management$DocumentEditable,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'DocumentManager',
		'Update',"Entity",
		{
			Entity:Entity
		},
		);
},
}
//文档服务
//
export const Document={
//通过主键获取对象
//
Get(
	//主键
	//类型:ObjectKey_long
	Id: ObjectKey_long,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Document> {
	return _invoker(
		'Document',
		'Get',"Id",
		{
			Id:Id
		},
		);
},
//通过快速访问键值获取对象
//
GetByKey(
	//快速访问键值
	//类型:string
	Key: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Document> {
	return _invoker(
		'Document',
		'GetByKey',"",
		{
			Key:Key
		},
		);
},
//通过ID获取容器对象
//
LoadContainer(
	//容器对象ID
	//类型:long
	Key: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Common$Documents$Category> {
	return _invoker(
		'Document',
		'LoadContainer',"",
		{
			Key:Key
		},
		);
},
//通过关键字搜索对象
//
Search(
	//搜索参数
	//类型:SF.Sys.Entities.SearchArgument
	Arg: SF$Sys$Entities$SearchArgument,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Documents$Document> {
	return _invoker(
		'Document',
		'Search',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
ListItems(
	//Arg
	//类型:ListItemsArgument_Nullable_long
	Arg: ListItemsArgument_Nullable_long,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Documents$Document> {
	return _invoker(
		'Document',
		'ListItems',"Arg",
		{
			Arg:Arg
		},
		);
},
//
//
ListChildContainers(
	//Arg
	//类型:ListItemsArgument_Nullable_long
	Arg: ListItemsArgument_Nullable_long,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Common$Documents$Category> {
	return _invoker(
		'Document',
		'ListChildContainers',"Arg",
		{
			Arg:Arg
		},
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
	) : PromiseLike<SF$Sys$NetworkService$Metadata$Library> {
	return _invoker(
		'ServiceMetadata',
		'Json',"",
null,		);
},
}
