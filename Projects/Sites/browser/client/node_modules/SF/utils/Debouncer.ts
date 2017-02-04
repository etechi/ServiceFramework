
export default class Debouncer {
    private _timers: any = {};
    dispose() {
        this.clear();
    }
    clear() {
        for (var k in this._timers)
            clearTimeout(this._timers[k]);
        this._timers = {};
    }
    exec(action: () => void, timeout: number = 100, key: string="default") {
        var t = this._timers[key];
        if (t)
            clearTimeout(t);
        this._timers[key] = setTimeout(() => {
            delete this._timers[key];
            action();               
        }, timeout
        );
    }
}