import * as React from 'react'

export interface QuickSideBarProperty {
}

class QuickSideBar extends React.Component<QuickSideBarProperty, any> {
    constructor(props: QuickSideBarProperty) {
        super(props)
    }
    render() {
        return <div className="page-sidebar navbar-collapse collapse">
        </div>
    }
}

export default QuickSideBar
