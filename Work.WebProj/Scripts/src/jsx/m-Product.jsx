﻿var GridRow = React.createClass({
    mixins: [React.addons.LinkedStateMixin],
    getInitialState: function() {
        return {
        };
    },
    delCheck:function(i,chd){
        this.props.delCheck(i,chd);
    },
    modify:function(){
        this.props.updateType(this.props.primKey);
    },
    filter:function(value,LName){
        var Label="無";
        CommData[LName].forEach(function(object, i){
            if(value==object.val){
                Label=object.label;
            }
        })
        return Label;
    },
    render:function(){
        return (

                <tr>
                    <td className="text-center"><GridCheckDel iKey={this.props.ikey} chd={this.props.itemData.check_del} delCheck={this.delCheck} /></td>
                    <td className="text-center"><GridButtonModify modify={this.modify}/></td>
                    <td>{this.props.itemData.product_name}</td>
                    <td>{this.props.itemData.i_Hide ? <span className="label label-default">隱藏</span>:<span className="label label-primary">顯示</span>}</td>
                    <td>{this.filter(this.props.itemData.i_Lang,'LangData')}</td>
                </tr>
            );
        }
});

//主表單
var GirdForm = React.createClass({
    mixins: [React.addons.LinkedStateMixin],
    getInitialState: function() {
        return {
            gridData:{rows:[],page:1},
            fieldData:{},
            searchData:{title:null},
            edit_type:0,
            checkAll:false,
            temp_info:null
        };
    },
    getDefaultProps:function(){
        return{
            fdName:'fieldData',
            gdName:'searchData',
            apiPathName:gb_approot+'api/Product',
            initPathName:gb_approot+'Active/Product/aj_Init'

        };
    },
    componentWillMount:function(){
        //在輸出前觸發，只執行一次如果您在這個方法中呼叫 setState() ，會發現雖然 render() 再次被觸發了但它還是只執行一次。
    },
    componentDidMount:function(){
        //只在客戶端執行一次，當渲染完成後立即執行。當生命週期執行到這一步，元件已經俱有 DOM 所以我們可以透過 this.getDOMNode() 來取得 DOM 。
        //如果您想整和其他 Javascript framework ，使用 setTimeout, setInterval, 或者是發動 AJAX 請在這個方法中執行這些動作。
        this.queryGridData(1);
        //this.getAjaxInitData();//載入init資料
    },
    componentWillReceiveProps:function(nextProps){
        //當元件收到新的 props 時被執行，這個方法在初始化時並不會被執行。使用的時機是在我們使用 setState() 並且呼叫 render() 之前您可以比對 props，舊的值在 this.props，而新值就從 nextProps 來。
    },
    shouldComponentUpdate:function(nextProps,nextState){
        /*
        如同其命名，是用來判斷元件是否該更新，當 props 或者 state 變更時會再重新 render 之前被執行。這個方法在初始化時不會被執行，或者當您使用了 forceUpdate 也不會被執行。
        當你確定改變的 props 或 state 並不需要觸發元件更新時，在這個方法中適當的回傳 false 可以提升一些效能。

        shouldComponentUpdate: function(nextProps, nextState) {
            return nextProps.id !== this.props.id;
        }

        如果 shouldComponentUpdate 回傳 false 則 render() 就會完全被跳過直到下一次 state 改變，此外 componentWillUpdate 和 componentDidUpdate 將不會被觸發。
        當 state 產生異動，為了防止一些奇妙的 bug 產生，預設 shouldComponentUpdate 永遠回傳 true ，不過如果您總是使用不可變性(immutable)的方式來使用 state，並且只在 render 讀取它們那麼你可以複寫 shouldComponentUpdate
        或者是當效能遇到瓶頸，特別是需要處理大量元件時，使用 shouldComponentUpdate 通常能有效地提升速度。
        */
        return true;
    },
    componentWillUpdate:function(nextProps,nextState){
        /*
            當收到 props 或者 state 立即執行，這個方法在初始化時不會被執行，使用時機通常是在準備更新之前。
            注意您不能在這個方法中使用 this.setState()。如果您需要在修改 props 之後更新 state 請使用 componentWillReceiveProps 取代
        */
    },
    componentDidUpdate:function(prevProps, prevState){
        /*
            在元件更新之後執行。這個方法同樣不在初始化時執行，使用時機為當元件被更新之後需要執行一些操作。
        */
        //設定新增時的編輯器
    },
    componentWillUnmount:function(){
        //元件被從 DOM 卸載之前執行，通常我們在這個方法清除一些不再需要地物件或 timer。
    },
    handleSubmit: function(e) {
        e.preventDefault();

        this.state.fieldData.ProductStandard = this.refs.subGridForm.state.gridSubData;

        if(this.state.edit_type==1){
            jqPost(this.props.apiPathName,this.state.fieldData)
            .done(function(data, textStatus, jqXHRdata) {
                if(data.result){
                    tosMessage(null,'新增完成',1);
                    this.updateType(data.id);
                }else{
                    alert(data.message);
                }
            }.bind(this))
            .fail(function( jqXHR, textStatus, errorThrown ) {
                showAjaxError(errorThrown);
            });
        }
        else if(this.state.edit_type==2){
            jqPut(this.props.apiPathName,this.state.fieldData)
            .done(function(data, textStatus, jqXHRdata) {
                if(data.result){
                    tosMessage(null,'修改完成',1);

                }else{
                    alert(data.message);
                }
            }.bind(this))
            .fail(function( jqXHR, textStatus, errorThrown ) {
                showAjaxError(errorThrown);
            });
        };
        return;
    },
    deleteSubmit:function(e){

        if(!confirm('確定是否刪除?')){
            return;
        }

        var ids = [];
        for(var i in this.state.gridData.rows){
            if(this.state.gridData.rows[i].check_del){
                ids.push('ids='+this.state.gridData.rows[i].product_id);
            }
        }

        if(ids.length==0){
            tosMessage(null,'未選擇刪除項',2);
            return;
        }

        jqDelete(this.props.apiPathName + '?' + ids.join('&'),{})
        .done(function(data, textStatus, jqXHRdata) {
            if(data.result){
                tosMessage(null,'刪除完成',1);
                this.queryGridData(0);
            }else{
                alert(data.message);
            }
        }.bind(this))
        .fail(function( jqXHR, textStatus, errorThrown ) {
            showAjaxError(errorThrown);
        });
    },
    handleSearch:function(e){
        e.preventDefault();
        this.queryGridData(0);
        return;
    },
    delCheck:function(i,chd){

        var newState = this.state;
        this.state.gridData.rows[i].check_del = !chd;
        this.setState(newState);
    },
    checkAll:function(){

        var newState = this.state;
        newState.checkAll = !newState.checkAll;
        for (var prop in this.state.gridData.rows) {
            this.state.gridData.rows[prop].check_del=newState.checkAll;
        }
        this.setState(newState);
    },
    gridData:function(page){

        var parms = {
            page:0
        };

        if(page==0){
            parms.page=this.state.gridData.page;
        }else{
            parms.page=page;
        }

        $.extend(parms, this.state.searchData);

        return jqGet(this.props.apiPathName,parms);
    },
    queryGridData:function(page){
        this.gridData(page)
        .done(function(data, textStatus, jqXHRdata) {
            this.setState({gridData:data});
        }.bind(this))
        .fail(function(jqXHR, textStatus, errorThrown) {
            showAjaxError(errorThrown);
        });
    },
    insertType:function(){
        this.setState({edit_type:1,fieldData:{i_Lang:'zh-TW'}});
    },
    updateType:function(id){
        jqGet(this.props.apiPathName,{id:id})
        .done(function(data, textStatus, jqXHRdata) {
            this.setState({edit_type:2,fieldData:data.data});
        }.bind(this))
        .fail(function( jqXHR, textStatus, errorThrown ) {
            showAjaxError(errorThrown);
        });
    },
    noneType:function(){
        this.gridData(0)
        .done(function(data, textStatus, jqXHRdata) {
            this.setState({edit_type:0,gridData:data});
        }.bind(this))
        .fail(function(jqXHR, textStatus, errorThrown) {
            showAjaxError(errorThrown);
        });
    },
    changeFDValue:function(name,e){
        this.setInputValue(this.props.fdName,name,e);
    },
    changeGDValue:function(name,e){
        this.setInputValue(this.props.gdName,name,e);
    },
    setInputValue:function(collentName,name,e){

        var obj = this.state[collentName];
        if(e.target.value=='true'){
            obj[name] = true;
        }else if(e.target.value=='false'){
            obj[name] = false;
        }else{
            obj[name] = e.target.value;
        }
        this.setState({fieldData:obj});
    },
    getAjaxInitData:function(){
        jqGet(this.props.initPathName)
        .done(function(data, textStatus, jqXHRdata) {
            this.setState({temp_info:data.tmp_info});
        }.bind(this))
        .fail(function( jqXHR, textStatus, errorThrown ) {
            showAjaxError(errorThrown);
        });
    },
    render: function() {
        var outHtml = null;

        if(this.state.edit_type==0)
        {
            var searchData = this.state.searchData;

            outHtml =
            (
            <div>
                <ul className="breadcrumb">
                    <li><i className="fa-list-alt"></i> {this.props.MenuName}</li>
                </ul>
                <h3 className="title">
                    {this.props.Caption}
                </h3>
                <form onSubmit={this.handleSearch}>
                    <div className="table-responsive">
                        <div className="table-header">
                            <div className="table-filter">
                                <div className="form-inline">
                                        <div className="form-group">
                                            <label>語系</label> { }
                                            <select className="form-control input-sm"
                                                value={searchData.i_Lang}
                                                onChange={this.changeGDValue.bind(this,'i_Lang')}
                                            >
                                                <option value="">選擇語系</option>
                                                {
                                                    CommData.LangData.map(function(itemData,i) {
                                                        return <option key={itemData.val} value={itemData.val}>{itemData.label}</option>;
                                                    })
                                                }
                                            </select> { }
                                        </div> { }
                                    <div className="form-group">
                                        <label>標題</label> { }
                                        <input type="text" className="form-control input-sm"
                                        value={searchData.title}
                                        onChange={this.changeGDValue.bind(this,'title')}
                                        placeholder="請輸入關鍵字..." /> { }
                                        <button className="btn-primary btn-sm" type="submit"><i className="fa-search"></i> 搜尋</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <table>
                            <thead>
                                <tr>
                                    <th className="col-xs-1 text-center">
                                        <label className="cbox">
                                            <input type="checkbox" checked={this.state.checkAll} onChange={this.checkAll} />
                                            <i className="fa-check"></i>
                                        </label>
                                    </th>
                                    <th className="col-xs-1 text-center">修改</th>
                                    <th className="col-xs-4">產品名稱</th>
                                    <th className="col-xs-2">狀態</th>
                                    <th className="col-xs-2">語系</th>
                                </tr>
                            </thead>
                            <tbody>
                                {
                                this.state.gridData.rows.map(function(itemData,i) {
                                return <GridRow
                                key={i}
                                ikey={i}
                                primKey={itemData.product_id}
                                itemData={itemData}
                                delCheck={this.delCheck}
                                updateType={this.updateType}
                                category={this.state.category}
                                />;
                                }.bind(this))
                                }
                            </tbody>
                        </table>
                    </div>
                    <GridNavPage
                    StartCount={this.state.gridData.startcount}
                    EndCount={this.state.gridData.endcount}
                    RecordCount={this.state.gridData.records}
                    TotalPage={this.state.gridData.total}
                    NowPage={this.state.gridData.page}
                    onQueryGridData={this.queryGridData}
                    InsertType={this.insertType}
                    deleteSubmit={this.deleteSubmit}
                    />
                </form>
            </div>
            );
        }
        else if(this.state.edit_type==1 || this.state.edit_type==2)
        {
            var fieldData = this.state.fieldData;

            outHtml=(
            <div>
                <ul className="breadcrumb">
                    <li><i className="fa-list-alt"></i> {this.props.MenuName}</li>
                </ul>
                <h4 className="title">{this.props.Caption} 資料維護</h4>
                <div className="alert alert-warning"><p><strong className="text-danger">* </strong> 為必填項目。</p></div>
                <form className="form-horizontal" onSubmit={this.handleSubmit}>

                    <div className="form-group">
                        <label className="col-xs-1 control-label text-danger">* 語系</label>
                        <div className="col-xs-3">
                            <select className="form-control"
                                value={fieldData.i_Lang}
                                onChange={this.changeFDValue.bind(this,'i_Lang')}
                                required>
                                    {
                                        CommData.LangData.map(function(itemData,i) {
                                            return <option key={itemData.val} value={itemData.val}>{itemData.label}</option>;
                                        })
                                    }
                            </select>
                        </div>
                    </div>

                    <div className="form-group">
                        <label className="col-xs-1 control-label text-danger">* 產品名稱</label>
                        <div className="col-xs-5">
                            <input type="text"
                            className="form-control"
                            value={fieldData.product_name}
                            onChange={this.changeFDValue.bind(this,'product_name')}
                            maxLength="64"
                            required />
                        </div>
                        <small className="col-xs-4 help-inline">最多64字</small>
                    </div>



                    <div className="form-group">
                        <label className="col-xs-1 control-label">前台顯示</label>
                        <div className="col-xs-2">
                            <div className="radio-inline">
                                <label>
                                    <input type="radio"
                                            name="i_Hide"
                                            value={false}
                                            checked={fieldData.i_Hide===false}
                                            onChange={this.changeFDValue.bind(this,'i_Hide')}
                                            />
                                    <span>顯示</span>
                                </label>
                            </div>
                            <div className="radio-inline">
                                <label>
                                    <input type="radio"
                                            name="i_Hide"
                                            value={true}
                                            checked={fieldData.i_Hide===true}
                                            onChange={this.changeFDValue.bind(this,'i_Hide')}
                                    />
                                    <span>隱藏</span>
                                </label>
                            </div>
                        </div>
                        <label className="col-xs-1 control-label">列表排序</label>
                        <div className="col-xs-2">
                            <input type="number"
                                    className="form-control"
                                    value={fieldData.sort}
                                    onChange={this.changeFDValue.bind(this,'sort')}
                                    maxLength="64" />
                        </div>
                        <small className="col-xs-3 help-inline">數字越大越前面</small>
                    </div>

                    <div className="form-group">
                        <label className="col-xs-1 control-label">產品簡介</label>
                        <div className="col-xs-6">
                            <textarea type="date" className="form-control" rows="4"
                                value={fieldData.product_info}
                                onChange={this.changeFDValue.bind(this,'product_info')}
                                maxLength="250"/>
                        </div>
                        <div className="col-xs-5 text-danger">
                            最多250字
                        </div>
                    </div>

                    <div className="form-group">
                        <label className="col-xs-1 control-label">備註</label>
                        <div className="col-xs-6">
                            <textarea type="date" className="form-control" rows="4"
                                value={fieldData.product_note}
                                onChange={this.changeFDValue.bind(this,'product_note')}
                                maxLength="250"/>
                        </div>
                        <div className="col-xs-5 text-danger">
                            最多250字
                        </div>
                    </div>

                    <SubGirdForm
                    MainId={fieldData.product_id}
                    handleSubmit={this.handleSubmit}
                    ref="subGridForm" />

                    <div className="form-action">
                        <div className="col-xs-4 col-xs-offset-1">
                            <button type="submit" className="btn-primary"><i className="fa-check"></i> 儲存</button>
                            <button className="col-xs-offset-1" type="button" onClick={this.noneType}><i className="fa-times"></i> 回前頁</button>
                        </div>
                    </div>
                </form>
            </div>
                );
        }else{
            outHtml=(<span>No Page</span>);
        }

        return outHtml;
    }
});

//明細列表
var SubGirdForm = React.createClass({
    mixins: [React.addons.LinkedStateMixin,SortableMixin],
    getInitialState: function() {
        return {
            gridSubData:[],
            refreshFileList:false,
            newId:0
        };
    },
    getDefaultProps:function(){
        return{
            fdName:'fieldData',
            gdName:'searchData',
            apiSubPathName:gb_approot+'api/ProductStandard',
            getIdPathName:gb_approot+'Active/Product/aj_GetNewID'
        };
    },
    componentDidMount:function(){
        this.queryGridData(0);
        //Sortable.create(simpleList, { /* options */ });
    },
    handleSubmit: function(e) {
        e.preventDefault();
        this.props.handleSubmit(this.state.gridSubData,e);
        return;
    },
    deleteSubmit:function(e){

        if(!confirm('確定是否刪除?')){
            return;
        }

        var ids = [];
        for(var i in this.state.gridSubData){
            if(this.state.gridSubData.rows[i].check_del){
                ids.push('ids='+this.state.gridSubData[i].standard_id);
            }
        }

        if(ids.length==0){
            tosMessage(null,'未選擇刪除項',2);
            return;
        }

        jqDelete(this.props.apiSubPathName+'?' + ids.join('&'),{})
        .done(function(data, textStatus, jqXHRdata) {
            if(data.result){
                tosMessage(null,'刪除完成',1);
                this.queryGridData(0);
            }else{
                alert(data.message);
            }
        }.bind(this))
        .fail(function( jqXHR, textStatus, errorThrown ) {
            showAjaxError(errorThrown);
        });
    },
    handleSearch:function(e){
        e.preventDefault();
        this.queryGridData(0);
        return;
    },
    gridData:function(page){
        var parms = {
            main_id:this.props.MainId
        };

        return jqGet(this.props.apiSubPathName,parms);
    },
    queryGridData:function(page){
        this.gridData(page)
        .done(function(data, textStatus, jqXHRdata) {
            this.setState({gridSubData:data});
        }.bind(this))
        .fail(function(jqXHR, textStatus, errorThrown) {
            showAjaxError(errorThrown);
        });
    },
    changeFDValue:function(name,e){
        this.setInputValue(this.props.fdName,name,e);
    },
    changeGDValue:function(name,e){
        this.setInputValue(this.props.gdName,name,e);
    },
    setSubInputValue:function(i,name,e){
        var obj = this.state.gridSubData[i];
        if(e.target.value=='true'){
            obj[name] = true;
        }else if(e.target.value=='false'){
            obj[name] = false;
        }else{
            obj[name] = e.target.value;
        }
        this.setState({fieldData:obj});
    },
    creatNewData:function(e){
        var newState = this.state;
        jqGet(this.props.getIdPathName,{})
        .done(function(data, textStatus, jqXHRdata) {
            newState.newId=data;
            var newData = {
                standard_id:this.state.newId,
                product_id:this.props.MainId,
                item_no:null,
                appearance:null,
                viscosity:null,
                soften_point:null,
                remark:null,
                sort:0,
                edit_state:0
            };
            newState.gridSubData.push(newData);
            this.setState(newState);
        }.bind(this));
    },
    deleteItem:function(i){
        var newState = this.state;
        var data = newState.gridSubData[i];

        if(data.edit_state==0){
            newState.gridSubData.splice(i,1);
            this.setState(newState);
        }else{
            jqDelete(this.props.apiSubPathName+'?ids=' + data.standard_id,{})
            .done(function(data, textStatus, jqXHRdata) {
                if(data.result){
                    newState.gridSubData.splice(i,1);
                    this.setState(newState);
                }else{
                    tosMessage(null,data.message,1);
                }
            }.bind(this))
            .fail(function(jqXHR, textStatus, errorThrown) {
                showAjaxError(errorThrown);
            });
        }
    },
    handleSort: function (evt) {
        var newState = this.state;
        var n = newState.gridSubData.length;
        for (var i in newState.gridSubData) {
            newState.gridSubData[i].sort=i;//排序改成數字越小越前面
            //n--;
        }
        newState.refreshFileList = true;
        this.setState(newState);
        this.setState({refreshFileList:false});
        //this.reloadFileList();
    },
    sortableOptions: {
        ref: "SortForm",
        model:'gridSubData',
        group: "shared",
        handle: ".fa-bars",
        ghostClass: "ghost"
    },
    reloadFileList:function (){
        //console.log(this.refs.reloadFileList.length);
        //this.refs.reloadFileList.reloadFileList();
    },
    render: function() {
        var outHtml = null;
        var fieldData = this.state.fieldData;
        outHtml=
        (
            <section className="format">
                <h4>產品規格列表</h4>

                <div className="alert alert-warning">
                    <button type="button" className="close" data-dismiss="alert"><span aria-hidden="true">×</span></button>
                    <ol>
                        <li>點選 <strong className="fa-bars"></strong> 並<strong>拖曳</strong>，可修改排列順序。</li>
                        <li>點選 <strong className="fa-times"></strong> 可刪除。</li>
                    </ol>
                </div>

                <table>
                    <tr>
                        <th></th>
                        <th>刪除</th>
                        <th className="text-danger text-center">* Item no. <small>(最多10字)</small></th>
                        <th className="text-danger text-center">* Appearance <small>(最多50字)</small></th>
                        <th className="text-danger text-center">Viscosity <small>(最多50字)</small></th>
                        <th className="text-danger text-center">Soften Point <small>(最多50字)</small></th>
                        <th className="text-center">Remark <small>(最多200字)</small></th>
                    </tr>

                    <tbody ref="SortForm">
                    {
                        this.state.gridSubData.map(function(itemData,i) {
                            return <SubGirdField key={itemData.standard_id} ikey={i} fieldData={itemData}
                            SetSubInputValue={this.setSubInputValue}
                            DeleteItem={this.deleteItem}
                            refreshFileList={this.state.refreshFileList}/>;
                        }.bind(this))
                    }
                    </tbody>
                </table>

                <button className="btn btn-add" type="button" onClick={this.creatNewData.bind(this)}><i className="fa-plus"></i> 新增規格列表</button>

            </section>
        );
        return outHtml;
    }
});

//明細表單
var SubGirdField = React.createClass({
    mixins: [React.addons.LinkedStateMixin],
    getInitialState: function() {
        return {
            fieldData:this.props.fieldData
        };
    },
    getDefaultProps:function(){
        return{
            subHtml:null

        };
    },
    componentDidMount:function(){
        if(this.state.fieldData.edit_state==0){
            this.state.fieldData.sort=this.props.ikey;//排序預設跟資料順序一樣
        }
    },
    componentWillReceiveProps:function(nextProps){
        this.state.fieldData = nextProps.fieldData;
    },
    changeFDValue:function(name,e){
        this.props.SetSubInputValue(this.props.ikey,name,e);
    },
    deleteItem:function(i){
        if(this.props.fieldData.edit_state==1){
            if(confirm('此筆資料已存在，確認是否刪除?')){
                this.props.DeleteItem(i);
            }
        }else{
            this.props.DeleteItem(i);
        }
    },
    render: function() {
        var outHtml = null;
        var fieldData = this.state.fieldData;

        outHtml = (
            <tr>
                <td className="text-center"><i className="fa-bars text-muted draggable"></i></td>
                <td className="text-center">
                    <button className="btn-link text-danger" title="刪除" onClick={this.deleteItem.bind(this,this.props.ikey)}><i className="fa-times"></i></button>
                </td>
                <td>
                    <input type="text"
                    className="form-control"
                    value={fieldData.item_no}
                    onChange={this.changeFDValue.bind(this,'item_no')}
                    maxLength="10"
                    required />
                </td>
                <td>
                    <input type="text"
                    className="form-control"
                    value={fieldData.appearance}
                    onChange={this.changeFDValue.bind(this,'appearance')}
                    maxLength="50"
                    required />
                </td>
                <td>
                    <input type="text"
                    className="form-control"
                    value={fieldData.viscosity}
                    onChange={this.changeFDValue.bind(this,'viscosity')}
                    maxLength="50"/>
                </td>
                <td>
                    <input type="text"
                    value={fieldData.soften_point}
                    onChange={this.changeFDValue.bind(this,'soften_point')}
                    maxLength="50" /> ℃
                </td>
                <td className="text-left">
                    <input type="text"
                    className="form-control"
                    value={fieldData.remark}
                    onChange={this.changeFDValue.bind(this,'remark')}
                    maxLength="200" />
                </td>
            </tr>
        );
        return outHtml;
    }
});