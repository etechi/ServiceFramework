export interface IMenuItem {
    path: string;
    name: string;
    icon: string;
}
export interface IMenuCategory {
    group?: string;
    name: string;
    icon: string;
    path: string;
    items: IMenuItem[];
}