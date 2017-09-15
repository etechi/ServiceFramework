var dom = document.getElementById("waiting");

export interface IWaitCover{
    start():void;
    show():void;
    error(e:any,hide:()=>void,retry?:()=>void):void;
    hide():void;
}
var _cover:IWaitCover=null; 
export function setup(cover:IWaitCover){
    _cover=cover;
}

export function start<T>(process: () => PromiseLike<T>,noRetry?:boolean) {
    if(!_cover)
        return process();
    _cover.start();
    return new Promise((resolve,reject)=>{
        var timer= setTimeout(() => {
            timer = null;
            _cover.show();
        }, 1000);
        var run=()=>{
            process().then(re => {
                 if (timer)
                 {  
                    clearTimeout(timer);
                    timer=null;
                 }
                
                 _cover.hide();
                 resolve(re);
            },
            e=>{
                 if (timer)
                 {  
                    clearTimeout(timer);
                    timer=null;
                 }
                 if(!e._netDown)
                 {
                    _cover.hide();
                    reject(e);
                    return;
                }
                _cover.error(e._error || "网络故障，请稍后再试...",()=>{
                    _cover.hide();
                    reject(e);
                },!noRetry?()=>{
                    _cover.show();
                    run();
                 }:null);
            });
        };
            run()
    });
   
}