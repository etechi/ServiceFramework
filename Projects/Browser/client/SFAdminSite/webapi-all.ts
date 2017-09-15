
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
	query?:any
}
export interface IApiInvoker{
	(type: string,method: string,query: { [index: string]: any},post: { [index: string]: any}, opts?: ICallOptions) :any
}

var _invoker:IApiInvoker=null;
export function setApiInvoker(invoker:IApiInvoker){
	_invoker=invoker;
}


// 
export interface SF$Core$Logging$LoggerSettingType {
	//
	//类型:long
	LogService?: number;
}
// 
export interface SF$Core$Caching$SystemMemoryCacheSettingType {
}
// 
export interface SF$Core$Times$TimeServiceSettingType {
}
// 
export interface SF$Core$TaskServices$TaskServiceManagerSettingType {
	//
	//类型:long
	ServiceProvider?: number;
}
// 
export interface SF$KB$Mime$Providers$DefaultMimeResolverSettingType {
}
// 
export interface SF$Core$Drawing$dotNetFramework$ImageProcessorSettingType {
}
// 
export interface SF$Data$Storage$DataContextSettingType {
	//
	//类型:long
	ProviderFactory?: number;
}
// 
export interface SF$Data$Storage$DataSetSettingType {
	//
	//类型:long
	Context?: number;
}
// 
export interface System$Data$Common$DbConnection extends System$ComponentModel$Component {
	//
	//类型:string
	ConnectionString?: string;
	//
	//类型:int
	ConnectionTimeout: number;
	//
	//类型:string
	Database?: string;
	//
	//类型:string
	DataSource?: string;
	//
	//类型:string
	ServerVersion?: string;
	//
	//类型:System.Data.ConnectionState
	State: System$Data$ConnectionState;
}
// 
export interface System$ComponentModel$Component {
}
// 
export type System$Data$ConnectionState = 'Closed'|'Open'|'Connecting'|'Executing'|'Fetching'|'Broken';
export const System$Data$ConnectionStateNames={
	"Closed":"Closed",
	"Open":"Open",
	"Connecting":"Connecting",
	"Executing":"Executing",
	"Fetching":"Fetching",
	"Broken":"Broken",
}
// 
export interface SF$Data$Storage$TransactionScopeManagerSettingType {
	//
	//类型:System.Data.Common.DbConnection
	Connection: System$Data$Common$DbConnection;
}
// 
export interface SF$Core$ServiceFeatures$ServiceFeatureControlServiceSettingType {
	//
	//类型:long
	ServiceProvider?: number;
}
// 
export interface SF$AdminSite$CalcSettingType {
}
// 
export interface SF$Core$ServiceManagement$ImplementConfigTypeSourceSettingType {
	//
	//类型:long
	Metadata?: number;
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
	//路径解析器
	//类型:long
	PathResolver?: number;
}
// 
export interface SF$Core$Caching$LocalFileCacheSettingType {
	//
	//类型:SF.Core.Caching.LocalFileCacheSetting
	Setting: SF$Core$Caching$LocalFileCacheSetting;
}
// 
export interface SF$Services$Media$MediaMetaCacheSettingType {
}
// 
export interface SF$Services$Media$MediaManagerSetting {
	//存储类型
	//类型:SF.Services.Media.MediaStorageSetting[]
	Types: SF$Services$Media$MediaStorageSetting[];
	//媒体元信息缓存
	//类型:long
	MetaCache?: number;
}
// 
export interface SF$Services$Media$MediaStorageSetting {
	//媒体标识类型
	//类型:string
	Type?: string;
	//媒体存储器
	//类型:long
	Storage?: number;
}
// 
export interface SF$Services$Media$MediaManagerSettingType {
	//
	//类型:SF.Services.Media.MediaManagerSetting
	Setting: SF$Services$Media$MediaManagerSetting;
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
export interface SF$AdminSite$AddSettingType {
}
// 
export interface SF$AdminSite$SubstractSettingType {
}
// 
export interface SF$AdminSite$AggConfig {
	//操作1
	//类型:long
	Op?: number;
	//增加
	//类型:int
	Add: number;
}
// 
export interface SF$AdminSite$AggSettingType {
	//操作
	//类型:long
	op?: number;
	//设置
	//类型:SF.AdminSite.AggConfig
	cfg: SF$AdminSite$AggConfig;
}
// 
export interface SF$Services$Test$TestServiceSettingType {
}
// 
export interface SF$Core$ServiceManagement$Management$ServiceDeclarationManagerSettingType {
	//
	//类型:long
	ServiceMetadata?: number;
}
// 
export interface SF$Core$ServiceManagement$Management$ServiceInstanceManagerSettingType {
	//
	//类型:long
	EntityResolver?: number;
	//
	//类型:long
	ConfigChangedNotifier?: number;
	//
	//类型:long
	ServiceProvider?: number;
	//
	//类型:long
	IdentGenerator?: number;
	//
	//类型:long
	TimeService?: number;
	//
	//类型:long
	Metadata?: number;
}
// 
export interface SF$Data$IdentGenerator$StorageIdentGeneratorSettingType {
	//预分配数量
	//类型:int
	CountPerBatch: number;
}
// 
export interface Lazy_IDataSet_SF$Management$MenuServices$Entity$DataModels$MenuItem {
	//
	//类型:bool
	IsValueCreated?: boolean;
}
// 
export interface Lazy_SF$Core$ServiceManagement$IServiceInstanceDescriptor {
	//
	//类型:bool
	IsValueCreated?: boolean;
}
// 
export interface SF$Management$MenuServices$Entity$EntityMenuServiceSettingType {
	//
	//类型:Lazy_IDataSet_SF.Management.MenuServices.Entity.DataModels.MenuItem
	MenuItemSet: Lazy_IDataSet_SF$Management$MenuServices$Entity$DataModels$MenuItem;
	//
	//类型:long
	TimeService?: number;
	//
	//类型:long
	IdentGenerator?: number;
	//
	//类型:Lazy_SF.Core.ServiceManagement.IServiceInstanceDescriptor
	ServiceInstanceDescriptor: Lazy_SF$Core$ServiceManagement$IServiceInstanceDescriptor;
}
// 
export interface SF$AdminSite$Controllers$AdminControllerSettingType {
}
// 
export interface SF$AdminSite$Controllers$HomeControllerSettingType {
}
// 
export interface SF$AdminSite$Api$UserControllerSettingType {
	//
	//类型:long
	calc?: number;
}
// 
export interface SF$Core$NetworkService$DefaultServiceBuildRuleProviderSettingType {
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
export interface SF$Core$NetworkService$DefaultServiceMetadataServiceSettingType {
	//
	//类型:SF.Core.NetworkService.Metadata.Library
	lib: SF$Core$NetworkService$Metadata$Library;
}
// 
export interface SF$Core$NetworkService$UploadedFileCollectionSettingType {
}
// 
export interface SF$Core$ServiceManagement$Models$ServiceDeclaration {
	//ID
	//类型:string
	Id?: string;
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
	//类型:string
	Id?: string;
	//服务定义名称
	//类型:string
	Name?: string;
	//服务定义分类
	//类型:string
	CategoryId?: string;
}
// 
export interface QueryResult_SF$Core$ServiceManagement$Models$ServiceDeclaration {
	//
	//类型:SF.Data.ISummary
	Summary?: SF$Data$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Core.ServiceManagement.Models.ServiceDeclaration[]
	Items?: SF$Core$ServiceManagement$Models$ServiceDeclaration[];
}
// 
export interface SF$Data$ISummary {
}
// 
export interface SF$Core$ServiceManagement$Models$ServiceImplement {
	//ID
	//类型:string
	Id?: string;
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
	//类型:string
	Id?: string;
	//服务实现名称
	//类型:string
	Name?: string;
	//服务实现分类
	//类型:string
	CategoryId?: string;
	//服务定义
	//类型:string
	DeclarationId?: string;
}
// 
export interface QueryResult_SF$Core$ServiceManagement$Models$ServiceImplement {
	//
	//类型:SF.Data.ISummary
	Summary?: SF$Data$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Core.ServiceManagement.Models.ServiceImplement[]
	Items?: SF$Core$ServiceManagement$Models$ServiceImplement[];
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
	ServiceType: string;
	//服务名称
	//类型:string
	ServiceName?: string;
	//服务实现
	//类型:string
	ImplementType: string;
	//服务实现
	//类型:string
	ImplementName?: string;
	//父服务实例
	//类型:long
	ParentId?: number;
	//父服务
	//类型:string
	ParentName?: string;
}
// 
export interface SF$Core$ServiceManagement$Models$ServiceInstance extends UIEntityBase_long {
	//服务标识
	//类型:string
	ServiceIdent?: string;
	//优先级
	//类型:int
	Priority: number;
}
// 
export interface UIEntityBase_long extends EntityBase_long {
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
export interface EntityBase_long {
	//Id
	//类型:long
	Id: number;
	//名称
	//类型:string
	Name: string;
	//对象状态
	//类型:SF.Data.LogicObjectState
	ObjectState: SF$Data$LogicObjectState;
	//创建时间
	//类型:datetime
	CreatedTime: string;
	//修改时间
	//类型:datetime
	UpdatedTime: string;
}
// 
export type SF$Data$LogicObjectState = 'Enabled'|'Disabled'|'Deleted';
export const SF$Data$LogicObjectStateNames={
	"Enabled":"有效",
	"Disabled":"无效",
	"Deleted":"已删除",
}
// 
export interface SF$Core$ServiceManagement$Management$ServiceInstanceQueryArgument {
	//ID
	//类型:long
	Id?: number;
	//服务实例名称
	//类型:string
	Name?: string;
	//服务定义
	//类型:string
	ServiceType?: string;
	//服务实现
	//类型:string
	ImplementId?: string;
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
	//类型:SF.Data.ISummary
	Summary?: SF$Data$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Core.ServiceManagement.Models.ServiceInstanceInternal[]
	Items?: SF$Core$ServiceManagement$Models$ServiceInstanceInternal[];
}
// 菜单项
export interface SF$Management$MenuServices$Models$MenuItem extends UIEntityBase_long {
	//字体图标
	//类型:string
	FontIcon?: string;
	//动作
	//类型:SF.Management.MenuServices.Models.MenuItemAction
	Action: SF$Management$MenuServices$Models$MenuItemAction;
	//动作参数
	//类型:string
	ActionArgument?: string;
	//子菜单
	//类型:SF.Management.MenuServices.Models.MenuItem[]
	Children?: SF$Management$MenuServices$Models$MenuItem[];
	//
	//类型:long
	ParentId?: number;
}
// 
export type SF$Management$MenuServices$Models$MenuItemAction = 'None'|'EntityManager'|'Form'|'List'|'IFrame'|'Link';
export const SF$Management$MenuServices$Models$MenuItemActionNames={
	"None":"无",
	"EntityManager":"实体管理",
	"Form":"显示表单",
	"List":"显示列表",
	"IFrame":"显示内嵌网页",
	"Link":"打开链接",
}
// 
export interface SF$Management$MenuServices$Models$Menu extends EntityBase_long {
	//菜单引用标识
	//类型:string
	Ident: string;
}
// 
export interface SF$Management$MenuServices$MenuQueryArgument {
	//
	//类型:long
	Id?: number;
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
	//类型:SF.Data.ISummary
	Summary?: SF$Data$ISummary;
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
	//类型:SF.Management.MenuServices.Models.MenuItem[]
	Items?: SF$Management$MenuServices$Models$MenuItem[];
}
//
//
export const ServiceFeatureControl={
//
//
Init(
	__opts?:ICallOptions
	) : PromiseLike<string> {
	return _invoker(
		'ServiceFeatureControl',
		'Init',
		null,
		null,
		__opts
		);
},
}
//
//
export const Calc={
//
//
Add(
	//
	//类型:int
	a: number,
	//
	//类型:int
	b: number,
	__opts?:ICallOptions
	) : PromiseLike<number> {
	return _invoker(
		'Calc',
		'Add',
		{
			a:a,
			b:b
		},
		null,
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
//测试服务
//
export const Test={
//
//
Test1(
	__opts?:ICallOptions
	) : PromiseLike<any> {
	return _invoker(
		'Test',
		'Test1',
		null,
		null,
		__opts
		);
},
//
//
Test2(
	__opts?:ICallOptions
	) : PromiseLike<any> {
	return _invoker(
		'Test',
		'Test2',
		null,
		null,
		__opts
		);
},
}
//服务定义管理
//定义系统内置服务
export const ServiceDeclarationManager={
//
//
Get(
	//
	//类型:string
	Id: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Core$ServiceManagement$Models$ServiceDeclaration> {
	return _invoker(
		'ServiceDeclarationManager',
		'Get',
		{
			Id:Id
		},
		null,
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
}
//服务实现管理
//系统内置服务实现
export const ServiceImplementManager={
//
//
Get(
	//
	//类型:string
	Id: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Core$ServiceManagement$Models$ServiceImplement> {
	return _invoker(
		'ServiceImplementManager',
		'Get',
		{
			Id:Id
		},
		null,
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
}
//服务实例管理
//系统内置服务实例
export const ServiceInstanceManager={
//
//
LoadForEdit(
	//
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Core$ServiceManagement$Models$ServiceInstanceEditable> {
	return _invoker(
		'ServiceInstanceManager',
		'LoadForEdit',
		{
			Id:Id
		},
		null,
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
	) : PromiseLike<number> {
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
//
//
Remove(
	//
	//类型:long
	Key: number,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ServiceInstanceManager',
		'Remove',
		{
			Key:Key
		},
		null,
		__opts
		);
},
//
//
Get(
	//
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Core$ServiceManagement$Models$ServiceInstanceInternal> {
	return _invoker(
		'ServiceInstanceManager',
		'Get',
		{
			Id:Id
		},
		null,
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
	) : PromiseLike<SF$Management$MenuServices$Models$MenuItem[]> {
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
//
//
Get(
	//
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$MenuServices$Models$Menu> {
	return _invoker(
		'Menu',
		'Get',
		{
			Id:Id
		},
		null,
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
LoadForEdit(
	//
	//类型:long
	Id: number,
	__opts?:ICallOptions
	) : PromiseLike<SF$Management$MenuServices$Models$MenuEditable> {
	return _invoker(
		'Menu',
		'LoadForEdit',
		{
			Id:Id
		},
		null,
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
	) : PromiseLike<number> {
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
//
//
Remove(
	//
	//类型:long
	Key: number,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'Menu',
		'Remove',
		{
			Key:Key
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
