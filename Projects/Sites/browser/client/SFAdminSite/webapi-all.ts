
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
export interface SF$Services$ManagedServices$Models$ServiceDeclaration {
	//
	//类型:string
	Id?: string;
	//
	//类型:string
	Name?: string;
	//
	//类型:string
	Description?: string;
	//
	//类型:string
	Group?: string;
	//
	//类型:bool
	Disabled?: boolean;
}
// 
export interface SF$Services$ManagedServices$Admin$ServiceDeclarationQueryArgument {
	//
	//类型:string
	Id?: string;
	//
	//类型:string
	Name?: string;
	//
	//类型:string
	CategoryId?: string;
}
// 
export interface SF$Data$Paging {
	//
	//类型:int
	Offset: number;
	//
	//类型:int
	Count: number;
	//
	//类型:string
	SortMethod?: string;
	//
	//类型:SF.Data.SortOrder
	SortOrder: SF$Data$SortOrder;
	//
	//类型:bool
	TotalRequired?: boolean;
	//
	//类型:bool
	SummaryRequired?: boolean;
}
// 
export type SF$Data$SortOrder = 'Default'|'Asc'|'Desc'|'Random';
export const SF$Data$SortOrderNames={
	"Default":"Default",
	"Asc":"Asc",
	"Desc":"Desc",
	"Random":"Random",
}
// 
export interface QueryResult_SF$Services$ManagedServices$Models$ServiceDeclaration {
	//
	//类型:SF.Data.ISummary
	Summary?: SF$Data$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Services.ManagedServices.Models.ServiceDeclaration[]
	Items?: SF$Services$ManagedServices$Models$ServiceDeclaration[];
}
// 
export interface SF$Data$ISummary {
}
// 
export interface SF$Services$ManagedServices$Models$ServiceImplement {
	//
	//类型:string
	Id?: string;
	//
	//类型:string
	Name?: string;
	//
	//类型:string
	Description?: string;
	//
	//类型:string
	Group?: string;
	//
	//类型:bool
	Disabled?: boolean;
	//
	//类型:string
	DeclarationId?: string;
	//
	//类型:string
	DeclarationName?: string;
}
// 
export interface SF$Services$ManagedServices$Admin$ServiceImplementQueryArgument {
	//
	//类型:string
	Id?: string;
	//
	//类型:string
	Name?: string;
	//
	//类型:string
	CategoryId?: string;
	//
	//类型:string
	DeclarationId?: string;
}
// 
export interface QueryResult_SF$Services$ManagedServices$Models$ServiceImplement {
	//
	//类型:SF.Data.ISummary
	Summary?: SF$Data$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Services.ManagedServices.Models.ServiceImplement[]
	Items?: SF$Services$ManagedServices$Models$ServiceImplement[];
}
// 
export interface SF$Services$ManagedServices$Models$ServiceInstance {
	//
	//类型:string
	Id?: string;
	//
	//类型:string
	Name?: string;
	//
	//类型:string
	DeclarationId?: string;
	//
	//类型:string
	DeclarationName?: string;
	//
	//类型:string
	ImplementId?: string;
	//
	//类型:string
	ImplementName?: string;
	//
	//类型:SF.Data.LogicObjectState
	LogicState: SF$Data$LogicObjectState;
	//
	//类型:string
	Config?: string;
}
// 
export type SF$Data$LogicObjectState = 'Enabled'|'Disabled'|'Deleted';
export const SF$Data$LogicObjectStateNames={
	"Enabled":"有效",
	"Disabled":"无效",
	"Deleted":"已删除",
}
// 
export interface SF$Services$ManagedServices$Admin$ServiceInstanceQueryArgument {
	//
	//类型:string
	Id?: string;
	//
	//类型:string
	Name?: string;
	//
	//类型:string
	DeclarationId?: string;
	//
	//类型:string
	ImplementId?: string;
}
// 
export interface QueryResult_SF$Services$ManagedServices$Models$ServiceInstance {
	//
	//类型:SF.Data.ISummary
	Summary?: SF$Data$ISummary;
	//
	//类型:int
	Total?: number;
	//
	//类型:SF.Services.ManagedServices.Models.ServiceInstance[]
	Items?: SF$Services$ManagedServices$Models$ServiceInstance[];
}
// 
export interface SF$Services$NetworkService$Metadata$Library extends SF$Metadata$Models$Library {
	//
	//类型:SF.Services.NetworkService.Metadata.Service[]
	Services?: SF$Services$NetworkService$Metadata$Service[];
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
export interface SF$Services$NetworkService$Metadata$Service extends SF$Metadata$Models$Entity {
	//
	//类型:SF.Services.NetworkService.Metadata.Method[]
	Methods?: SF$Services$NetworkService$Metadata$Method[];
	//
	//类型:SF.Services.NetworkService.Metadata.GrantInfo
	GrantInfo?: SF$Services$NetworkService$Metadata$GrantInfo;
}
// 
export interface SF$Services$NetworkService$Metadata$Method extends SF$Metadata$Models$Method {
	//
	//类型:string
	HeavyParameter?: string;
	//
	//类型:SF.Services.NetworkService.Metadata.GrantInfo
	GrantInfo?: SF$Services$NetworkService$Metadata$GrantInfo;
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
export interface SF$Services$NetworkService$Metadata$GrantInfo {
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
//
//
export const ServiceDeclarationManager={
//
//
Load(
	//
	//类型:string
	Id: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Services$ManagedServices$Models$ServiceDeclaration> {
	return _invoker(
		'ServiceDeclarationManager',
		'Load',
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
	//类型:SF.Services.ManagedServices.Admin.ServiceDeclarationQueryArgument
	Arg: SF$Services$ManagedServices$Admin$ServiceDeclarationQueryArgument,
	//
	//类型:SF.Data.Paging
	paging: SF$Data$Paging,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Services$ManagedServices$Models$ServiceDeclaration> {
	return _invoker(
		'ServiceDeclarationManager',
		'Query',
		{
			Arg:Arg
		},
		paging,
		__opts
		);
},
}
//
//
export const ServiceImplementManager={
//
//
Load(
	//
	//类型:string
	Id: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Services$ManagedServices$Models$ServiceImplement> {
	return _invoker(
		'ServiceImplementManager',
		'Load',
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
	//类型:SF.Services.ManagedServices.Admin.ServiceImplementQueryArgument
	Arg: SF$Services$ManagedServices$Admin$ServiceImplementQueryArgument,
	//
	//类型:SF.Data.Paging
	paging: SF$Data$Paging,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Services$ManagedServices$Models$ServiceImplement> {
	return _invoker(
		'ServiceImplementManager',
		'Query',
		{
			Arg:Arg
		},
		paging,
		__opts
		);
},
}
//
//
export const ServiceInstanceManager={
//
//
LoadForUpdate(
	//
	//类型:string
	Id: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Services$ManagedServices$Models$ServiceInstance> {
	return _invoker(
		'ServiceInstanceManager',
		'LoadForUpdate',
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
	//类型:SF.Services.ManagedServices.Models.ServiceInstance
	Entity: SF$Services$ManagedServices$Models$ServiceInstance,
	__opts?:ICallOptions
	) : PromiseLike<string> {
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
	//类型:SF.Services.ManagedServices.Models.ServiceInstance
	Entity: SF$Services$ManagedServices$Models$ServiceInstance,
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
Delete(
	//
	//类型:string
	Key: string,
	__opts?:ICallOptions
	) : PromiseLike<void> {
	return _invoker(
		'ServiceInstanceManager',
		'Delete',
		{
			Key:Key
		},
		null,
		__opts
		);
},
//
//
Load(
	//
	//类型:string
	Id: string,
	__opts?:ICallOptions
	) : PromiseLike<SF$Services$ManagedServices$Models$ServiceInstance> {
	return _invoker(
		'ServiceInstanceManager',
		'Load',
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
	//类型:SF.Services.ManagedServices.Admin.ServiceInstanceQueryArgument
	Arg: SF$Services$ManagedServices$Admin$ServiceInstanceQueryArgument,
	//
	//类型:SF.Data.Paging
	paging: SF$Data$Paging,
	__opts?:ICallOptions
	) : PromiseLike<QueryResult_SF$Services$ManagedServices$Models$ServiceInstance> {
	return _invoker(
		'ServiceInstanceManager',
		'Query',
		{
			Arg:Arg
		},
		paging,
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
	) : PromiseLike<SF$Services$NetworkService$Metadata$Library> {
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
