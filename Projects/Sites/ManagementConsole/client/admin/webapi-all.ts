
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
export interface ManagementConsole$PostArgument {
	//
	//类型:string
	arg?: string;
}
// 
export interface SF$Services$Metadata$Models$Library extends SF$Metadata$Models$Library {
	//
	//类型:SF.Services.Metadata.Models.Service[]
	Services?: SF$Services$Metadata$Models$Service[];
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
export interface SF$Services$Metadata$Models$Service extends SF$Metadata$Models$Entity {
	//
	//类型:SF.Services.Metadata.Models.Method[]
	Methods?: SF$Services$Metadata$Models$Method[];
	//
	//类型:SF.Services.Metadata.Models.GrantInfo
	GrantInfo?: SF$Services$Metadata$Models$GrantInfo;
}
// 
export interface SF$Services$Metadata$Models$Method extends SF$Metadata$Models$Method {
	//
	//类型:string
	HeavyParameter?: string;
	//
	//类型:SF.Services.Metadata.Models.GrantInfo
	GrantInfo?: SF$Services$Metadata$Models$GrantInfo;
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
export interface SF$Services$Metadata$Models$GrantInfo {
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
export const Add={
//
//
Calc(
	//
	//类型:int
	a: number,
	//
	//类型:int
	b: number,
	//
	//类型:ManagementConsole.PostArgument
	pa: ManagementConsole$PostArgument,
	__opts?:ICallOptions
	) : PromiseLike<number> {
	return _invoker(
		'Add',
		'Calc',
		{
			a:a,
			b:b
		},
		pa,
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
	) : PromiseLike<SF$Services$Metadata$Models$Library> {
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
