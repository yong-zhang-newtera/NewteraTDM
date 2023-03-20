
ASPxClientGridView = _aspxCreateClass(ASPxClientControl, {
	constructor: function(name){
		this.constructor.prototype.constructor.call(this, name);
		this.callBacksEnabled = true;
        this.custwindowLeft = null;
        this.custwindowTop = null;
        this.custwindowVisible = null;
        this.activeElement = null;
        this.filterKeyPressInputValue = "";
        this.userChangedSelection = false;
        this.lockFilter = false;
        this.confirmDelete = "";
        this.filterKeyPressTimerId = -1;
        this.virtualScrollTimerId = -1;
        this.lockVirtualScrolling = false;
        this.savedScrollPosition = 0;
        this.lastMultiSelectIndex = -1;
        this.filterPopupWindow = null;
        this.filterPopupActiveColumnIndex = -1;
        this.filterRowMenuColumnIndex = -1;
        this.editorIDList = [ ];
        this.keys = [];
        this.isPagerUiCallback = false;    
        this.SelectionChanged = new ASPxClientEvent();    
        this.FocusedRowChanged = new ASPxClientEvent();  
        this.ColumnSorting = new ASPxClientEvent();    
        this.ColumnGrouping = new ASPxClientEvent();
        this.ColumnMoving = new ASPxClientEvent();    
        this.ColumnStartDragging  = new ASPxClientEvent();    
        this.ColumnResizing  = new ASPxClientEvent();    
        this.RowExpanding  = new ASPxClientEvent();    
        this.RowCollapsing  = new ASPxClientEvent();    
        this.DetailRowExpanding  = new ASPxClientEvent();    
        this.DetailRowCollapsing  = new ASPxClientEvent();    
        this.RowClick  = new ASPxClientEvent();    
        this.RowDblClick  = new ASPxClientEvent();    
        this.ContextMenu = new ASPxClientEvent();    
        this.CustomizationWindowCloseUp = new ASPxClientEvent();
        this.CustomButtonClick = new ASPxClientEvent();

        this.funcCallbacks = new Array();

        //set from server
		this.pageRowCount = 0;
		this.pageRowSize = 0;
        this.allowFocusedRow = false;
        this.allowMultiSelection = false;
        this.focusedRowIndex = -1;
        this.selectedWithoutPageRowCount = 0;
        this.visibleStartIndex = 0;
        this.columns = new Array();
        this.isColumnsResizable = false;
        this.isMainControlResizable = false;
        this.isVerticalScrolling = false;
        this.isHorizontalScrolling = false;
        this.fixedColumnCount = 0;
        this.scrollToRowIndex = -1;
        this.virtualScrollRowHeight = 0;
        this.callbackOnFocusedRowChanged = false;
        this.callbackOnSelectionChanged = false;
        this.autoFilterDelay = 1200;
        
        if(ASPxClientGridView.instances)
			ASPxClientGridView.instances[this.name] = this;
	},
	ClearColumns: function() {
        this.columns = new Array();
	},
    CreateColumn: function (name, index, fieldName, visible) {
        if(!_aspxIsExists(this.columns)) 
            this.columns = new Array();
        this.columns.push(new ASPxClientGridViewColumn(name, index, fieldName, visible));
    },
	
	_constDXDataRow: function() { return "DXDataRow"; },
	_constDXGroupRow: function() { return "DXGroupRow"; },
	_constDXCustWindow: function() { return "_DXTDcustwindow"; },
	_constDXEmptyHeader: function() { return "_DXTDemptyheader"; },
	_constDXDropTarget: function() { return "_DXT"; },
	_constDXDropSource: function() { return "_DXTD"; },
	_constDXCanGroup: function() { return "_DXTDG"; },
	_constDXMainTable: function() { return "DXMainTable"; },
	_isGroupRow: function(row) { return row.id.indexOf(this._constDXGroupRow()) > -1; },
	GetRootTable: function() { return _aspxGetElementById(this.name); },
	GetGridTD: function() { 
	    var table = this.GetRootTable();
	    if(!_aspxIsExists(table)) return null;
	    return table.rows[0].cells[0];
	},
    GetArrowDragDownImage: function() { return this.GetChildElementById("IADD"); },
    GetArrowDragUpImage: function() { return this.GetChildElementById("IADU"); },
    GetArrowDragFieldImage: function() { return this.GetChildElementById("IDHF"); },
    GetCallbackState: function() { return this.GetChildElementById("CallbackState"); },
	GetSelectionInput: function() { return this.GetChildElementById("DXSelInput"); },
	GetFocusedRowInput: function() { return this.GetChildElementById("DXFocusedRowInput"); },
	GetColResizedInput: function() { return this.GetChildElementById("DXColResizedInput"); },
    GetLoadingPanelDiv: function() {  return this.GetChildElementById("LPD"); },
    GetHorzScrollDiv: function() {  return this.GetChildElementById("DXHorzScrollDiv"); },
    GetFixedColumnsDiv: function() {  return this.GetChildElementById("DXFixedColumnsDiv"); },
    GetRow: function(visibleIndex) { 
        var res = this.GetDataRow(visibleIndex);
        if(res == null) res = this.GetGroupRow(visibleIndex);
        return res;
    },
	GetDataRow: function(visibleIndex) { return this.GetChildElementById(this._constDXDataRow() + visibleIndex); },
	GetGroupRow: function(visibleIndex) { 
		var element = this.GetChildElementById(this._constDXGroupRow() + visibleIndex);
		if(!element)
			element = this.GetExpandedGroupRow(visibleIndex);
		return element; 
	},
	GetExpandedGroupRow: function(visibleIndex) { return this.GetChildElementById(this._constDXGroupRow() + "Exp" + visibleIndex); },
    GetDataRowSelBtn: function(index) { return this.GetChildElementById("DXSelBtn" + index); },
	GetMainTable: function() { return this.GetChildElementById(this._constDXMainTable()); },
	GetStyleTable: function() { return this.GetChildElementById("DXStyleTable"); },
	GetLoadingPanelContainer: function() { return this.GetChildElementById("DXLPContainer"); },
    IsGroupHeader: function(id) { return id.indexOf("_DXTDgroup") > 0; },
    GetHeadersRow: function() { 
        return this.GetChildElementById("DXHeadersRow"); 
    },
    GetEditingRow: function(obj) { return _aspxGetElementById(obj.name + "_DXEditingRow"); },
    GetEditingErrorRow: function(obj) { return _aspxGetElementById(obj.name + "_DXEditingErrorRow"); },
    GetEditFormTable: function() { return _aspxGetElementById(this.name + "_DXEFT"); },

	GetCustomizationWindow: function() { return aspxGetControlCollection().Get(this.name + this._constDXCustWindow()); },
	GetParentRowsWindow: function() { return aspxGetControlCollection().Get(this.name + "_DXparentrowswindow"); },
    GetEditorPrefix: function() { return "DXEditor"; },
    GetPopupEditForm: function() { return aspxGetControlCollection().Get(this.name  + "_DXPEForm"); },
    GetFilterWindowContent: function(){ return this.GetChildElementById("FPC"); },
    GetFilterRowMenu: function() { return aspxGetControlCollection().Get(this.name + "_DXFilterRowMenu"); },
    GetFilterControlPopup: function() { return aspxGetControlCollection().Get(this.name + "_DXPFCForm"); },
        GetFilterControl: function() { return aspxGetControlCollection().Get(this.name +  "_DXPFCForm_DXPFC"); }, 
    
    GetEditorByColumnIndex: function(colIndex) { 
        var list = this._getEditors();
        for(var i = 0; i < list.length; i++) {
            if(this._getNumberFromEndOfString(list[i].name) == colIndex) return list[i];
        }
        return null;
    },    
    Initialize: function(){
        this.constructor.prototype.Initialize.call(this);
        if(this.enabled)
			this.SetHeadersClientEvents();
        this._setFocusedRowInputValue();
        this.UpdateHorzScrollDivs();
        this.doScrollToRowIndex();
        this.restoreVirtualScrollPosition();
        this.setOnVirtualScroll();
    },
    AdjustControlCore: function() {
        ASPxClientControl.prototype.AdjustControlCore.call(this);
        this.UpdateVerticalScrollControls();
    },
    GetChildElementById: function(childName){
        if(!_aspxIsExists(this.childrenCache)) 
            this.childrenCache = new Object();
        if(!_aspxIsExistsElement(this.childrenCache[childName])) 
            this.childrenCache[childName] = _aspxGetElementById(this.name + "_" + childName);
        return this.childrenCache[childName];
    },
    gridCallBack: function(arguments) {
        if(!this.isInitialized)
            return;
        this.OnBeforeCallbackOrPostBack();
        var serializedArgs = _aspxSerializeStringArray(arguments); // B132204
        if(!_aspxIsExists(this.callBack) || !this.callBacksEnabled) {
            this.SendPostBack(serializedArgs);
        } else {
            this.OnBeforeCallback();
            var command = this.GetCorrectedCommand(arguments);
            var preparedArgs = this.prepareCallbackArgs(serializedArgs, this.GetGridTD());
            this.lockFilter = true;
            this.userChangedSelection = false;
            this.CreateCallback(preparedArgs, command);
        }
    },
    GetCorrectedCommand: function(arguments) {
        if(arguments.length == 0)
            return "";
        var command = arguments[0];
        if(arguments.length > 1 && command == "COLUMNMOVE") {
            if(arguments[arguments.length - 1] == "true")
                command = "UNGROUP";
            if(arguments[arguments.length - 2] == "true")
                command = "GROUP";
        }
        return command;
    },
	FuncGetCallBackIndex: function(onCallBack) {
	    for(var i = 0; i < this.funcCallbacks.length; i ++) {
	        if(this.funcCallbacks[i] == null) {
	            this.funcCallbacks[i] = onCallBack;
	            return i;
	        }
	    }
	    this.funcCallbacks.push(onCallBack);
	    return this.funcCallbacks.length - 1;
	},
	GetFuncCallBack: function(index) {
	    if(index < 0 || index >= this.funcCallbacks.length) return null;
	    var result = this.funcCallbacks[index];
	    this.funcCallbacks[index] = null;
	    return result;
	},
	gridFuncCallBack: function(arguments, onCallBack) {
        var serializedArgs = _aspxSerializeStringArray(arguments); // B132204
        var callbackArgs = this.formatCallbackArg("FB", this.FuncGetCallBackIndex(onCallBack).toString()) +
            this.prepareCallbackArgs(serializedArgs, null);
        this.CreateCallback(callbackArgs, "FUNCTION");
    },	
    prepareCallbackArgs: function(serializedArgs, rootTD) {
        var preparedArgs =
            this.formatCallbackArg("EV", this.GetEditorValues(rootTD)) +
            this.formatCallbackArg("SR", this.GetSelectedState()) +
            this.formatCallbackArg("FR", this.GetFocusedRowInput()) +
            this.formatCallbackArg("CR", this.GetColResizedInput()) +
            this.formatCallbackArg("GB", serializedArgs);
        return preparedArgs;
    },
    formatCallbackArg: function(prefix, arg) {
        if(arg == null) return "";
        if(!_aspxIsExists(arg.length) && _aspxIsExists(arg.value)) {
            arg = arg.value;
        }
        if(arg == null || arg == "") return "";
        return prefix + "|" + arg.length + ';' + arg + ';';
    },
	OnCallback: function(result){
		this.HideFilterControlPopup();
	    if(result.indexOf("FB|") == 0) {
	        _aspxSetTimeout("aspxGVFuncCallback(\""+this.name+"\", \"" + escape(result.substr(3)) +"\");", 0);
	    } else {
			// B35795
			var helper = this.getFixedColumnsHelper();
			if(helper != null) helper.hiddenColumnCount = 0;			
			
	        var rootTD = this.GetGridTD();
	        if(rootTD != null) {
	            _aspxSetInnerHtml(rootTD, result);
	            this.restoreVirtualScrollPosition();
	        }
            _aspxSetTimeout("aspxRestoreCallBackTimer(\""+this.name+"\");", 0);
	    }
	},
	OnFuncCallback: function(result) {
	    var pos = result.indexOf("|");
	    if(pos < 0) return;
	    var index = parseInt(result.substr(0, pos), 10);
	    var onCallBack = this.GetFuncCallBack(index);
	    if(onCallBack == null) return;
	    result = result.substr(pos + 1);
	    onCallBack(eval(result));
	},
	OnCallbackError: function(result, data){
	    var rootTD = this.GetGridTD();
	    this.showingError = result;
	    if(rootTD != null) 
            _aspxSetTimeout("aspxRestoreCallBackTimer(\"" + this.name + "\");", 0);
	},
	ShowError: function(errorText) {
	    var displayIn = this;
        if(_aspxIsExists(this.GetPopupEditForm())) {
            displayIn = this.GetPopupEditForm();
            if(!this.GetPopupEditForm().IsVisible()) {
                this.GetPopupEditForm().Show();	    
            }
        }
	    var errorRow = this.GetEditingErrorRow(displayIn);
	    if(errorRow == null) {
    	    var editRow = this.GetEditingRow(displayIn);
    	    if(editRow != null) {
            	    errorRow = editRow.parentNode.insertRow(editRow.sectionRowIndex + 1);
            	    errorRow.id = editRow.id.replace("DXEditingRow", "DXEditingErrorRow");
        	    this.ApplyEditingErrorRowStyle(errorRow);
    	    }
	    }
	    if(errorRow != null) {
	        errorRow.cells[errorRow.cells.length - 1].innerHTML = errorText;
	    } else {
	      alert(errorText);
	    }
	},
	ApplyEditingErrorRowStyle: function(errorRow) {
	    var row = this.GetStyleTable().rows[1];
	    errorRow.className = row.className;
	    errorRow.style.cssText = row.style.cssText;
	    for(var i = 0; i < row.cells.length; i ++) {
	        errorRow.appendChild(row.cells[i].cloneNode(true));
	    }
	},
	OnBeforeCallbackOrPostBack: function() {
	    this.lastMultiSelectIndex = -1;
	    this.HidePopupEditForm();
	},
	OnBeforeCallback: function() {
        _aspxClearTimer(this.virtualScrollTimerId);
        this.virtualScrollTimerId = -1;
	    this.CreateLoadingPanel(this.GetGridTD());
	    this.SaveCallbackSettings();
	},
	OnAfterCallback: function() {
	    if(_aspxIsExists(this.showingError) && this.showingError != "") {
  	        this.ShowError(this.showingError);
       	    this.showingError = null;
  	    }
	    this.childrenCache = new Object();
	    this.lockFilter = true;
	    try {
	        this.SetHeadersClientEvents();
	        this.UpdateHorzScrollDivs();
	        this.RestoreCallbackSettings();
	    }
	    finally {
	        this.lockFilter = false;
	    }
	},
	SaveCallbackSettings: function() {
	    this.saveScrollSettings();
	    var el = this.activeElement;
	    this.activeElement = null;
	    this.savedActiveElementId = null;
	    if(el != null && el.id && el.id.indexOf(this.name) == 0) {
	        this.SaveActiveElementSettings(el);
	    }
	    var custWindow = this.GetCustomizationWindow();
	    if(custWindow != null) {
            var custWindowElement = custWindow.GetWindowElement(-1);
            if(_aspxIsExists(custWindowElement )){
                this.custwindowLeft = _aspxGetAbsoluteX(custWindowElement) - _aspxGetIEDocumentClientOffset(true);
                this.custwindowTop = _aspxGetAbsoluteY(custWindowElement) - _aspxGetIEDocumentClientOffset(false);
                this.custwindowVisible = custWindow.IsVisible();
            }
	    } else {
    	    this.custwindowVisible = null;
	    }
	},
	RestoreCallbackSettings: function() {
	    var custWindow = this.GetCustomizationWindow();
	    if(custWindow != null && this.custwindowVisible != null) {
            if(this.custwindowVisible){
                custWindow.enableAnimation = false;
                custWindow.ShowAtPos(this.custwindowLeft, this.custwindowTop);
            }
	    }
	    this.RestoreActiveElementSettings();
	    this.UpdateVerticalScrollControls();
	    this.restoreScrollSettings();
	    this.sizeCorrectedOnce = false; // B96637
	    this.isPagerUiCallback = false;
	},
	SaveActiveElementSettings: function(element) {
	    if(element.tagName != "INPUT")
	        return;
	    this.savedActiveElementId = element.id;
	    this.savedActiveElementCaret = _aspxGetCaretEnd(element);
	},
	RestoreActiveElementSettings: function() {
	    if(this.savedActiveElementId == null) return;
	    var element = _aspxGetElementById(this.savedActiveElementId);
	    if(_aspxIsExists(element)) {
	        element.focus();
	        _aspxSetCaret(element, this.savedActiveElementCaret);
	    }
	    this.savedActiveElementId = null;
	},
	HidePopupEditForm: function() {
	    var popup = this.GetPopupEditForm();
	    if(popup != null) {
            popup.CloseUp.ClearHandlers();
            popup.Hide();
        }
	},
	_isRowSelected: function(visibleIndex) {
	    if(!_aspxIsExists(this.GetDataRow(visibleIndex))) return false;
	    var index = this._getRowIndexOnPage(visibleIndex);
	    var selInput = this.GetSelectionInput();
	    if(!_aspxIsExists(selInput)) return false;
	    return this._isTrueInCheckList(selInput.value, index);
	},
	_isTrueInCheckList: function(checkList, index) {
        if(index < 0 ||  index >= checkList.length) return false;
        return checkList.charAt(index) == "T";
	},
	_getSelectedRowCount: function() {
	    var res = this.selectedWithoutPageRowCount;
	    var selInput = this.GetSelectionInput();
	    if(!_aspxIsExists(selInput)) return res;
        var checkList = selInput.value;
        var selCount = 0;
        for(var i = 0; i < checkList.length; i++) {
            if(checkList.charAt(i) == "T") selCount ++;
        }
        return res + selCount;
	},
	_selectAllRowsOnPage: function(checked) {
	    var selInput = this.GetSelectionInput();
	    if(!_aspxIsExists(selInput)) return;
        this._selectAllSelBtn(checked);
	    for(var i = 0; i < this.pageRowCount; i ++) {
	        if(this._isTrueInCheckList(selInput.value, i) != checked) {
	            this.ChangeRowStyle(i + this.visibleStartIndex, checked ? 0 : 3);
	        }
	    }
	    var selValue = "";
	    if(checked) {
	        for(var i = 0; i < this.pageRowCount; i ++)
	            selValue += this.IsDataRow(this.visibleStartIndex + i ) ? "T" : "F";
	    }
	    if(selValue != selInput.value) {
    	    this.userChangedSelection = true;
    	    if(selValue == "") selValue = "U";
	        selInput.value = selValue;
	        //todo fire event
	    }
	    this.DoSelectionChanged(-1, checked, true);
	},
	DeleteGridRow: function(visibleIndex) {
	    if(this.confirmDelete != "" && !confirm(this.confirmDelete)) return;
	    this.DeleteRow(visibleIndex);
	},
	_selectAllSelBtn: function(checked) {
	    if(this.pageRowCount <= 0) return;
	    for(var i = 0; i < this.pageRowCount; i ++) {
	        var element = this.GetDataRowSelBtn(i + this.visibleStartIndex);
	        if(element != null) {
	            element.checked = checked;
	        }
	    }
	},
	SelectRowCore: function(visibleIndex, check) {
	    var checked = false;
	    var isCheckBox = _aspxIsExists(check) && _aspxIsExists(check.checked) && _aspxIsExists(check.type) && check.type.toLowerCase() == "checkbox";
	    if(!isCheckBox) {
	        checked = !this._isRowSelected(visibleIndex);
	    } else {
	        checked = check.checked;
	    }
        this.SelectRow(visibleIndex, checked, isCheckBox);
    },
    doRowMultiSelect: function(row, rowIndex, evt) {
		var ctrlKey = evt.ctrlKey || evt.metaKey;
        _aspxClearSelection();
        if(!ctrlKey && !evt.shiftKey) {
            this._selectAllRowsOnPage(false);
            this.SelectRow(rowIndex, true);
            this.lastMultiSelectIndex = rowIndex;
        } else {
            if(ctrlKey) {
                this.SelectRow(rowIndex, !this._isRowSelected(rowIndex));
                this.lastMultiSelectIndex = rowIndex;
            } else {
                var startIndex = rowIndex > this.lastMultiSelectIndex ? this.lastMultiSelectIndex + 1 : rowIndex;
                var endIndex = rowIndex > this.lastMultiSelectIndex ? rowIndex : this.lastMultiSelectIndex - 1;
                for(var i = this.visibleStartIndex; i < this.pageRowCount + this.visibleStartIndex; i ++) {
                    if(i == this.lastMultiSelectIndex) continue;
                    this.SelectRow(i, i >= startIndex && i <= endIndex);
                }
            }
        }
    },
	SelectRow: function(visibleIndex, checked, fromCheckBox) {
	    if(!this.IsDataRow(visibleIndex)) return;
	    if(_aspxIsExists(fromCheckBox)) fromCheckBox = false;
	    var index = this._getRowIndexOnPage(visibleIndex);
	    if(index < 0) return;
	    var selInput = this.GetSelectionInput();
	    if(_aspxIsExists(selInput)) {
	        this.userChangedSelection = true;
	        var checkList = selInput.value;
	        if(index >= checkList.length) {
	            if(!checked) return;
	            for(var i = checkList.length; i <= index; i ++)
	                checkList += "F";
	        }
	        checkList = checkList.substr(0, index) + (checked ? "T" : "F") + checkList.substr(index + 1, checkList.length - index - 1);
	        if(checkList.indexOf("T") < 0) checkList = "U";
	        selInput.value = checkList;
	    }
	    if(!fromCheckBox) {
	        var checkBox = this.GetDataRowSelBtn(visibleIndex);
	        if(checkBox != null) checkBox.checked = checked;
	    }
	    this.ChangeRowStyle(visibleIndex, checked ? 0 : 3);
	    this.DoSelectionChanged(visibleIndex, checked, false);
	},
	getRowByHtmlEvent: function(evt) {
	    var row = _aspxGetParentByPartialId(_aspxGetEventSource(evt), this._constDXDataRow());
	    if(!_aspxIsExists(row))
	        row = _aspxGetParentByPartialId(_aspxGetEventSource(evt), this._constDXGroupRow());
	    return row;
	},
	mainTableClick: function(evt) { this.getGridByRow(evt).mainTableClickCore(evt); },
	mainTableClickCore: function(evt) {
	    var row = this.getRowByHtmlEvent(evt);
	    if(_aspxIsExists(row)) {
	        var rowIndex = this.getRowIndex(row.id);
	        if(this.RaiseRowClick(rowIndex, evt)) return;    
    	    if(this.allowFocusedRow) {
	            this.focusRow(row);
	        }
	        if(this.allowMultiSelection) {
	            if(_aspxGetEventSource(evt) != this.GetDataRowSelBtn(rowIndex)) {
	                this.doRowMultiSelect(row, rowIndex, evt);
	            }
	        }
	    }
	},
	mainTableDblClick: function(evt) { this.getGridByRow(evt).mainTableDblClickCore(evt); },
	mainTableDblClickCore: function(evt) {
	    var row = this.getRowByHtmlEvent(evt);
	    if(_aspxIsExists(row)) {
	        this.RaiseRowDblClick(this.getRowIndex(row.id), evt);    
	    }
	},
	getGridByRow: function(rowEvt) {
	    var row = this.getRowByHtmlEvent(rowEvt);
	    if(!_aspxIsExists(row)) return this;
	    var id = row.offsetParent.id;
	    id = id.substr(0, id.length - this._constDXMainTable().length - 1);
	    var table = aspxGetControlCollection().Get(id);
	    return _aspxIsExists(table) ? table : this;
    },
	focusRow: function(row) {
	    if(!_aspxIsExists(row)) return;
	    var index = this.getRowIndex(row.id);
	    this._setFocusedRowIndex(index);
	},
	_setFocusedRowIndex: function(visibleIndex) {
		if(visibleIndex < 0) visibleIndex = -1;
	    if(!this.allowFocusedRow) return;
	    var row = this.GetRow(visibleIndex);
	    if((visibleIndex > -1 && row == null) || visibleIndex == this.focusedRowIndex) return;
	    var oldFocusedRow = this.focusedRowIndex;
	    this.focusedRowIndex = visibleIndex;
	    this.ChangeRowStyle(oldFocusedRow, this._isRowSelected(oldFocusedRow) ? 0 : 3);
	    if(this.focusedRowIndex > -1)
	    this.ChangeRowStyle(this.focusedRowIndex, this._isGroupRow(row) ? 2 : 1);
	    this._setFocusedRowInputValue();
	    if(this.callbackOnFocusedRowChanged) {
	        this.gridCallBack(["FOCUSEDROW"]);
	        return;
	    }
        this.RaiseFocusedRowChanged();
	},
	_setFocusedRowInputValue: function() {
	    if(this.GetFocusedRowInput() != null) {
	        this.GetFocusedRowInput().value = this.focusedRowIndex;
	    }
	},
    _getFocusedRowIndex: function() {
	    if(!this.allowFocusedRow) return -1;
	    return this.focusedRowIndex;
    },
    getRowIndex: function(rowId) { return this._getNumberFromEndOfString(rowId); },
    _getNumberFromEndOfString: function(st) {
        var value = -1;
        var n = st.length - 1;
        while(parseInt(st.substr(n), 10) >= 0) {
            value = parseInt(st.substr(n), 10);
            n --;
        }
        return value;
    },
	GetSelectedState: function() {
	    if(!this.userChangedSelection) return null;
	    if(!_aspxIsExists(this.GetSelectionInput())) return null;
	    return this.GetSelectionInput().value;
	},
	//rowStyle -> 0 - selected, 1 - focused Row, 2 - focused GroupRow, 3 - regular row
	ChangeRowStyle: function(visibleIndex, rowStyle) {
	    if(this._getFocusedRowIndex() == visibleIndex && rowStyle != 1 && rowStyle != 2) return;
	    var row = this.GetRow(visibleIndex);
	    if(!_aspxIsExists(row)) return;
	    if(rowStyle == 0 && this._isGroupRow(row)) return; // do not select the group row
	    var index = this._getRowIndexOnPage(visibleIndex);
	    var styleRow = this._getStyleRow(index, rowStyle);
	    if(!_aspxIsExists(styleRow)) return;
	    row.className = styleRow.className;
	    row.style.cssText = styleRow.style.cssText;
	},
	_getRowIndexOnPage: function(visibleIndex) { 
	    return visibleIndex - this.visibleStartIndex; 
	},
    getColumnIndex: function(colId) { 
        if(colId.indexOf(this._constDXEmptyHeader()) > -1) return 0;
        var index = this._getNumberFromEndOfString(colId); 
        return colId.indexOf("col" + index) > -1 ? index : -1;
    },
    getColumnObject: function(colId) {
        var index = this.getColumnIndex(colId);
        return index > -1 ? this._getColumn(index) : null;
    },
    _getColumnIndexByColumnArgs: function(column) {
        column = this._getColumnObjectByArg(column);
        if(!_aspxIsExists(column)) return null;
        return column.index;
    },
    _getColumnObjectByArg: function(arg) {
        if(!_aspxIsExists(arg)) return null;
        if(typeof(arg) == "number") return this._getColumn(arg);
        if(_aspxIsExists(arg.index)) return arg;
        var column = this._getColumnById(arg);
        if(_aspxIsExists(column)) return column;
        return this._getColumnByField(arg);        
    },
    _getColumnsCount: function() { return this.columns.length; },
    _getColumn: function(index) { 
        if(index < 0 || index >= this.columns.length) return null;
        return this.columns[index];
    },
    _getColumnById: function(id) {
        if(!_aspxIsExists(id)) return null;
        for(var i = 0; i < this.columns.length; i++) {
            if(this.columns[i].id == id) return this.columns[i];
        }
        return null;
    },
    _getColumnByField: function(fieldName) {
        if(!_aspxIsExists(fieldName)) return null;
        for(var i = 0; i < this.columns.length; i++) {
            if(this.columns[i].fieldName == fieldName) return this.columns[i];
        }
        return null;
    },
	//0 - selected, 1 - focused data row,  2  - focused group row, 3 - regular
	_getStyleRow: function(index, rowStyle) {
	    var styleTable = this.GetStyleTable();
	    if(!_aspxIsExists(styleTable)) return null;
	    if(rowStyle < 3) return styleTable.rows[rowStyle + 2];
	    return styleTable.rows[5 + index];
	},
    DoSelectionChanged: function(index, isSelected, isSelectAllOnPage){
        if(this.callbackOnSelectionChanged) {
            this.gridCallBack(["SELECTION"]);
            return;
        }
        this.RaiseSelectionChanged(index, isSelected, isSelectAllOnPage, false);
    },
    CommandCustomButton:function(id, index) {
		var processOnServer = true;
		if(!this.CustomButtonClick.IsEmpty()) {
			var e = new ASPxClientGridViewCustomButtonEventArgs(index, id);
			this.CustomButtonClick.FireEvent(this, e);
			processOnServer = e.processOnServer;
		}
		if(processOnServer)
			this.gridCallBack(["CUSTOMBUTTON", id, index]);
    },
    HeaderColumnResizing: function(e){
        var element = _aspxGetEventSource(e);
        if(!_aspxIsExists(element)) return;
        if(!_aspxIsExistsType(typeof(ASPxClientTableColumnResizing))) return;
        if(!_aspxIsExists(this.columnResizing)) {
            this.columnResizing = new ASPxClientTableColumnResizing(this, "DXMainTable", "DXHeaderTable", "DXFooterTable", this.isHorizontalScrolling);
            this.columnResizing.CanResizeLastColumn = this.isMainControlResizable;
        }
        this.columnResizing.UpdateCursor(element, e , this.indentColumnCount);
    },
	HeaderMouseDown: function(element, e){
        if(!_aspxGetIsLeftButtonPressed(e)) return;
        var source = _aspxGetEventSource(e);
        if(_aspxIsExists(source.onclick))
            return;
        if(this.startColumnResizing(element, e)) return;

        var column = this._getColumnObjectByArg(this.getColumnIndex(element.id));
        if(this.RaiseColumnStartDragging(column)) return;

        var canDrag = (element.id.indexOf(this._constDXDropSource()) > -1) && (source.tagName!="IMG");
	    var drag = this.createDrag(e, element, canDrag);
        this.createTargets(drag, e);
	},
	createDrag: function(e, element, canDrag) {
	    var drag = new ASPxClientDragHelper(e, element, true);
	    drag.canDrag = canDrag;
	    drag.onDragDivCreating = this.DragDivCreating;
	    drag.grid = this;
	    drag.ctrl = aspxGetCtrlKey(e);
	    drag.shift = aspxGetShiftKey(e);
        drag.onDoClick = this.headerDoClick;
        drag.onCloneCreating = this.cloneCreating;
        drag.onEndDrag = this.endDrag;
        drag.onCancelDrag = this.cancelDrag;
        return drag;
	},	
	createTargets: function(drag, e) {
	    if(!drag.canDrag) return;
        var targets = new ASPxClientCursorTargets();
        targets.obj = drag.obj;
        targets.grid = this;
        targets.onTargetChanging = this.targetChanging; 
        targets.onTargetChanged = this.targetChanged; 
        targets.onTargetAdding = this.targetAdding;
        var targertIds = new Array();
        targertIds.push(this._constDXCustWindow());
        targertIds.push(this._constDXDropTarget());
        targets.RegisterTargets(this.GetRootTable(), targertIds);
        targets.removeInitialTarget(e.clientX, e.clientY);
	},
	startColumnResizing: function(element, e) {
        if(!_aspxIsExists(this.columnResizing)) return false;
        if(!this.columnResizing.CanStartResizing(element, e, this.indentColumnCount)) return false;
        
        var column = this._getColumnObjectByArg(this.getColumnIndex(element.id));
        if(this.RaiseColumnResizing(column)) return false;

        this.columnResizing.StartResizing(element, e, this.indentColumnCount);
        this.columnResizing.SetResizedInput(this.GetColResizedInput());
        if(this.isMainControlResizable) {
            this.columnResizing.SetResizableControl(this.GetRootTable());
        }

        var editorsToBeResized = [];
        var currentColumnAutoFilterEditor = this.GetAutoFilterEditor(column);
        if(currentColumnAutoFilterEditor)
            editorsToBeResized.push(currentColumnAutoFilterEditor);
        editorsToBeResized = editorsToBeResized.concat(this._getEditors());
        this.columnResizing.SetEditorsToBeResized(editorsToBeResized);

        return true;
	},
	DragDivCreating: function(drag, dragDiv) {
	    var rootTable = drag.grid.GetRootTable();
	    if(!_aspxIsExists(dragDiv) || !_aspxIsExists(rootTable)) return;
	    dragDiv.className = rootTable.className;
	    dragDiv.style.cssText = rootTable.style.cssText;
	},
    headerDoClick: function(drag) {
        if(!drag.grid.getIsColumnCanSort(drag.obj)) return;
        var order = "";
        drag.grid.SortBy(drag.grid.getColumnIndex(drag.obj.id), drag.ctrl ? "NONE" : "", !drag.shift && !drag.ctrl);
    },
    cancelDrag: function(drag) {
        drag.grid.targetImagesChangeVisibility("hidden");
    },
    endDrag: function(drag) {
        if(drag.targetElement == null) return;
        var grid = drag.grid;        
        var column = grid.getColumnIndex(drag.obj.id);
        var id = drag.targetElement.id;
        var columnTo = grid.getColumnIndex(id);
        var isLeft = grid.IsRightToLeft() ? !drag.targetTag : drag.targetTag;
        grid.MoveColumn(column, columnTo, isLeft, grid.IsGroupHeader(id), grid.IsGroupHeader(drag.obj.id));
    },
    cloneCreating: function(clone) {
        var table = document.createElement("table");
        table.width = this.obj.offsetWidth + "px";
        var row = table.insertRow(-1);
        clone.style.borderLeftWidth = __aspxOpera ? "0px" : "";
        clone.style.borderTopWidth = "";
        clone.style.borderRightWidth = __aspxOpera ? "0px" : "";
        row.appendChild(clone);
        return table;
    },
    IsElementBelongsToGrid: function(elementId) {
        var id = elementId.toLowerCase();
        var gridName = this.name.toLowerCase();
        var pos = id.indexOf(gridName);
        if(pos < 0) return false;
        id = id.substr(pos + gridName.length);
        return id.indexOf("_detailgrid_") < 0;
    },
    targetChanging: function(targets) {
        targets.targetTag = targets.isLeftPartOfElement();
        var grid = targets.grid;
        if(grid.IsTagertElementGroupPanel(targets)) {
            targets.targetTag = true;
            if(!grid.getIsColumnCanGroup(targets.obj)) {
                targets.targetElement = null;
            }
        }
        if(targets.targetElement != null) {
            grid.RaiseColumnMoving(targets);
        }
    },
    targetAdding: function(targets, element) {
        var grid = targets.grid;
        if(!grid.IsElementBelongsToGrid(element.id)) return false;
        var hScrollCtrl =  grid.getHorzScrollableControl();
        if(!_aspxIsExists(hScrollCtrl)) return true;
        if(grid.getColumnIndex(element.id) < 0 || grid.IsGroupHeader(element.id)) return true;
        var x1 = _aspxGetAbsoluteX(element);
        var x2 = _aspxGetAbsoluteX(hScrollCtrl);
        return x1 >= x2 && x1 + element.offsetWidth <= x2 + hScrollCtrl.offsetWidth;
    },
    IsTagertElementGroupPanel: function(targets) {
        return targets.targetElement != null && targets.targetElement.id.indexOf("grouppanel") > -1;
    },
    targetChanged: function(targets) {
       if(__aspxDragHelper == null) return;
       if(targets.targetElement == __aspxDragHelper.obj) return;
       if(targets.targetElement != null) {
            __aspxDragHelper.targetElement = targets.targetElement;
            __aspxDragHelper.targetTag = targets.targetTag;
            var left = _aspxGetAbsoluteX(targets.targetElement);
            if(targets.targetTag == false) {
                left += targets.targetElement.offsetWidth;
            }
            targets.grid.setDragImagesPosition(targets.targetElement, left);
        } else {
            __aspxDragHelper.targetElement = null;
            targets.grid.targetImagesChangeVisibility("hidden");
        }
    },
    targetImagesChangeVisibility: function(vis) {
        if(this.GetArrowDragDownImage() == null) return;
        this.GetArrowDragDownImage().style.visibility = vis;
        this.GetArrowDragUpImage().style.visibility = vis;
        if(__aspxDragHelper != null) {
            __aspxDragHelper.removeElementFromDragDiv();
        }
    },
    setDragImagesPosition: function(el, left) {
        this.targetImagesChangeVisibility("hidden");
        if(el == this.getCustomizationWindowElement()) {
            __aspxDragHelper.addElementToDragDiv(this.GetArrowDragFieldImage());
        } else {
            left = _aspxPrepareClientPosForElement(left, this.GetArrowDragDownImage(), true);
            var top = _aspxPrepareClientPosForElement(_aspxGetAbsoluteY(el), this.GetArrowDragDownImage(), false);
            this.targetImagesChangeVisibility("visible");
            if(this.GetArrowDragDownImage() != null) {
                this.GetArrowDragDownImage().style.left = left + "px";
                this.GetArrowDragUpImage().style.left = left + "px";
                this.GetArrowDragDownImage().style.top = (top - this.GetArrowDragDownImage().offsetHeight) + "px";
                this.GetArrowDragUpImage().style.top = (top + el.offsetHeight) + "px";
            }
        }
    },
    getCustomizationWindowElement: function() {
        if(this.GetCustomizationWindow() != null) 
            return this.GetCustomizationWindow().GetWindowElement(-1);
        return null;
    },
    OnParentRowMouseEnter: function(element) {
        if(this.GetParentRowsWindow() == null) return;
        if(this.GetParentRowsWindow().IsWindowVisible()) return;
        this.ParentRowsTimerId = _aspxSetTimeout("aspxGVParentRowsTimer(\""+this.name+"\", \"" + element.id + "\");", 500);
    },
    OnParentRowMouseLeave: function(evt) {
        if(_aspxIsExists(this.ParentRowsTimerId)) {
            _aspxClearTimer(this.ParentRowsTimerId);
        }
        if(this.GetParentRowsWindow() == null) return;
        if(_aspxIsExists(evt) && _aspxIsExists(evt.toElement)) {
            if(_aspxGetParentByPartialId(evt.toElement, this.GetParentRowsWindow().name) != null)
              return;
        }
        this.HideParentRows();
    },
    ShowParentRows: function(element) {
        this.ParentRowsTimerId = null;
        if(this.GetParentRowsWindow() != null) {
            this.GetParentRowsWindow().ShowAtElement(element);
            //this.GetParentRowsWindow().ShowAtPos(_aspxGetAbsoluteX(element) + element.offsetWidth - 1, _aspxGetAbsoluteY(element) - 1);
        }
    },
    ShowFilterPopup: function(headerName, colIndex, htmlEvent) {
        if(this.filterPopupWindow == null) {
            this.filterPopupWindow = new ASPxClientPopupFilterWindow(this.name);
        }
        if(this.filterPopupWindow.IsShowing() && this.filterPopupActiveColumnIndex == colIndex) {
			this.HideFilterPopup();
			return;
		}
        var header = this.GetChildElementById(headerName);
        if(header == null) return;
        this.filterPopupWindow.Show(header, this.GetMainTable());
        this.filterPopupActiveColumnIndex = colIndex;
        this.gridFuncCallBack(["FILTERPOPUP", this.name, colIndex, (aspxGetShiftKey(htmlEvent) ? "T" : "")], this.onFilterPopupCallback);
        var content = this.GetFilterWindowContent();
        if(content != null) {
			content.innerHTML = "";
            var el = this.CreateLoadingPanelInsideContainer(content);
            _aspxRemoveBorders(el);
            var contentStyle = _aspxGetCurrentStyle(content);
            if(contentStyle != null) {
                _aspxSetBackground(el, contentStyle.backgroundColor);
            }
        }
    },
    HideFilterPopup: function() {
        if(this.filterPopupWindow == null) return;
        this.filterPopupWindow.Hide();
        this.filterPopupWindow = null;
        this.filterPopupActiveColumnIndex = -1;
    },
    onFilterItemOver: function(row) {
        row.oldclassName = row.className;
        row.oldcssText = row.style.cssText;
        var table = row.offsetParent;
        var selRow = table.rows[table.rows.length - 1];
        row.className = selRow.className;
        row.style.cssText = selRow.style.cssText;
    }, 
    onFilterItemOut: function(row) {
        row.className = row.oldclassName;
        row.style.cssText = row.oldcssText;
    }, 
    onFilterPopupCallback: function(values) {
        var grid = aspxGetControlCollection().Get(values[0]);
        if(grid == null) return;
        var fwContent = grid.GetFilterWindowContent();
        if(fwContent == null) return;
        _aspxSetInnerHtml(fwContent, values[1]);
        grid.CorrectFilterPopupPosition();
    },
    CorrectFilterPopupPosition: function() {
		if(!this.filterPopupWindow) return;		
		var win = this.filterPopupWindow.GetWindow();
		var left = win.offsetLeft;		
		win.style.left = "0px";		
		var width = win.offsetWidth;		
		var docWidth = __aspxIE && !__aspxIE7 
		    ? _aspxGetDocumentClientWidth()  // B132119
		    : _aspxGetDocumentWidth();
		var overhead = width - docWidth + left;
		if(overhead > 0) {
			left -= overhead;		
			if(left < 0) left = 0;
		}
		win.style.left = left + "px";
    },
    HideParentRows: function() {
        this.ParentRowsTimerId = null;
        if(this.GetParentRowsWindow() != null) {
            this.GetParentRowsWindow().Hide();
        }
    },
    getIsColumnCanSort: function(colElement) {
        return this.getIsColumnCanDoOperation(colElement, "S");
    },
    getIsColumnCanGroup: function(colElement) {
        return colElement.id.indexOf(this._constDXCanGroup()) > -1;
    },
    getIsColumnCanDoOperation: function(colElement, op) {
		var id = colElement.id;
		return id.substr(id.lastIndexOf("_")).indexOf(op) > -1;        
    },
    doPagerOnClick: function(id) {
        if(!_aspxIsExists(id)) return;
        this.isPagerUiCallback = true;
        this.gridCallBack(["PAGERONCLICK", id]);
    },
    OnColumnFilterInputChanged: function(editor) {
        this.ApplyColumnAutoFilterCore(editor);
    },
    OnColumnFilterInputSpecKeyPress: function(editor, e) {
        if(_aspxIsExists(e.htmlEvent)) e = e.htmlEvent;
        if(e.keyCode == 13) {
            e.cancelBubble = true;
            e.returnValue = false;
            editor.Validate();
            this.ApplyColumnAutoFilterCore(editor);
            return;
        }
        if(e.keyCode == 46 && e.ctrlKey) {
            e.cancelBubble = true;
            e.returnValue = false;
            editor.SetValue(null);
            this.ApplyColumnAutoFilterCore(editor);
            return;
        }
    },
    OnColumnFilterInputKeyPress: function(editor, e) {
        this.OnColumnFilterInputSpecKeyPress(editor, e);
        if(_aspxIsExists(e.htmlEvent)) e = e.htmlEvent;
        if(e.keyCode == 9) {
            e.cancelBubble = true;
        }
        if(e.keyCode == 13) return;
        if(e.keyCode == 46 && e.ctrlKey) return;
        this.ClearAutoFilterInputTimer();
        if(editor != this.FilterKeyPressEditor) {
            this.filterKeyPressInputValue = editor.GetValueString();
        }
        this.FilterKeyPressEditor = editor;
        this.filterKeyPressTimerId = _aspxSetTimeout("aspxGVTimer(\""+this.name+"\");", this.autoFilterDelay);
    },
    ClearAutoFilterInputTimer: function() {
        this.filterKeyPressTimerId = _aspxClearTimer(this.filterKeyPressTimerId);
    },
    GetAutoFilterEditorInputElement: function(editor) {
        if(_aspxIsExists(document.activeElement)) return document.activeElement;
        if(_aspxIsExists(editor.GetInputElement)) return editor.GetInputElement();
        return null;
    
    },
    OnFilterKeyPressTick: function() {
        if(_aspxIsExists(this.FilterKeyPressEditor)) {
            this.ApplyColumnAutoFilterCore(this.FilterKeyPressEditor);
        }
    },
    ApplyColumnAutoFilterCore: function(editor) {
        if(this.lockFilter) return;
        this.ClearAutoFilterInputTimer();
        if(_aspxIsExists(this.FilterKeyPressEditor) && editor == this.FilterKeyPressEditor) {
            if(this.FilterKeyPressEditor.GetValueString() == this.filterKeyPressInputValue) return;
        }
        var column = this.getColumnIndex(editor.name);
        if(column < 0) return;
        this.activeElement = this.GetAutoFilterEditorInputElement(editor);
        this.AutoFilterByColumn(column, editor.GetValueString());
    },
    FilterRowMenuButtonClick: function(columnIndex, element) {    
		var menu = this.GetFilterRowMenu();
		if(!menu) return;
		for(var i = menu.GetItemCount() - 1; i >= 0; i--) {
			var item = menu.GetItem(i);
			item.SetChecked(item.name == this.filterRowConditions[columnIndex]);		
		}
		menu.ShowAtElement(element);
		this.filterRowMenuColumnIndex = columnIndex;
    },
    FilterRowMenuItemClick: function(item) {
		this.gridCallBack(["FILTERROWMENU", this.filterRowMenuColumnIndex, item.name]);
    },
    
    //loading panel
    CreateLoadingPanel: function(content) {
        if(content == null) return;
        
        this.CreateLoadingDiv(content);
        var lpContainer = this.GetLoadingPanelContainer();
        if(lpContainer == null)
			this.CreateLoadingPanelWithAbsolutePosition(content);
	    else
			this.CreateLoadingPanelInline(lpContainer);
    },
    ///Editors
    _updateEdit: function() {
		if(window.ASPxClientEdit && !ASPxClientEdit.ValidateEditorsInContainer(this.GetEditFormTable(), this.name))
			return;
        var list = this._getEditors();
        if(list.length != 0) {
            if(!this._validate(list)) return;
        }
        this.gridCallBack(["UPDATEEDIT"]);
    },
    _validate: function(list) {
        var isValid = true;
        var firstInvalid = null;
        var edit;
        for(var i = 0; i < list.length; i ++) {
            edit = list[i];
            edit.Validate();
            isValid = edit.GetIsValid() && isValid;
            if(firstInvalid == null && edit.setFocusOnError && !edit.GetIsValid())
                firstInvalid = edit;
        }
        if (firstInvalid != null)
            firstInvalid.Focus();
        return isValid;
    },
    _getEditors: function() {
		var list = [ ];
		for(var i = 0; i < this.editorIDList.length; i++) {
			var editor = aspxGetControlCollection().Get(this.editorIDList[i]);
			if(editor && editor.GetMainElement && _aspxIsExistsElement(editor.GetMainElement())) {
				if(!_aspxIsExists(editor.Validate)) continue; //for image editors
				list.push(editor);
			}
        }
        return list;
    },
    GetEditorValues: function() {
        var list = this._getEditors();
        if(list.length == 0) return null;
        //if(!this.Validate(list)) return null;
        var res = list.length + ";";
        for(var i = 0; i < list.length; i ++) {
            res += this.GetEditorValue(list[i]);
        }
        return res;
    },
    GetEditorValue: function(editor) {
        var value = editor.GetValueString();
        var valueLength = -1;
        if(!_aspxIsExists(value)) {
            value = "";
        } else {
            value = value.toString();
            valueLength = value.length;
        }
        return this.GetEditorIndex(editor.name) + "," + valueLength + "," + value + ";";
    },
    GetEditorIndex: function(editorId) {
        var i = editorId.lastIndexOf(this.GetEditorPrefix());
        if(i < 0) return -1;
        return editorId.substr(i + this.GetEditorPrefix().length);
    },
    saveScrollSettings: function() {
        this.savedScrollPosition = this.getVertScrollPosition();
        this.savedHorzScrollPosition = this.getScrollPosition(this.getHorzScrollableControl(), false);
    },
    restoreScrollSettings: function() {
        this.restoreVerticalScrollPosition();
        if(_aspxIsExists(this.savedHorzScrollPosition)) {
            this.setScrollPosition(this.getHorzScrollableControl(), this.savedHorzScrollPosition, false);
        }
        this.setOnVirtualScroll();
    },
    restoreVerticalScrollPosition: function() {
        if((this.virtualScrollRowHeight <= 0) && !this.doScrollToRowIndex()) {
			if(this.isPagerUiCallback)
				this.savedScrollPosition = 0;
            this.setScrollPosition(this.getScrollableControl(), this.savedScrollPosition, true);
        }
    },
    restoreVirtualScrollPosition: function() {
        if(this.virtualScrollRowHeight <= 0) return false;
        this.updateVirtualMarginBottom();
        this.setScrollPosition(this.getScrollableControl(), this.getVirtualScrollMarginTop(), true);
        return true;
    },
    updateVirtualMarginBottom: function() {
        var table = this.GetMainTable();
        var parent = table.parentNode;
        var margin = null;
        for(var i = parent.childNodes.length - 1; i >= 0; i --){
            margin = parent.childNodes[i];
            if(_aspxIsExists(margin.offsetHeight)) break;
        }
        if(margin == this.GetMainTable()) return;
        if(_aspxPxToInt(margin.offsetHeight) <= 0) {
            var marginBottom = this.getScrollableControl().clientHeight - table.clientHeight;
            if(marginBottom > 0) {
                margin.style.height = marginBottom + "px";
            }
        }
    },
    getVirtualScrollMarginTop: function() {
        var parent = this.GetMainTable().parentNode;
        var margin = null;
        for(var i = 0; i < parent.childNodes.length; i ++) {
            margin = parent.childNodes[i];
            if(_aspxIsExists(margin.offsetHeight)) break;
        }
        if(margin == this.GetMainTable()) return 0;
        return margin.offsetHeight;
    },
    setOnVirtualScroll: function() {
        if(this.virtualScrollRowHeight <= 0) return;
        this.getScrollableControl().grid = this;
        _aspxAttachEventToElement(this.getScrollableControl(), "scroll", this.OnVirtualScroll);//this.getScrollableControl().onscroll = this.OnVirtualScroll;
        this.savedVirtualScroll = -1;
    },
    OnVirtualScroll: function(event) {
        var src = _aspxGetEventSource(event);
        var grid = src.grid;
        _aspxClearTimer(grid.virtualScrollTimerId);
        grid.savedVirtualScroll = src.scrollTop;
        grid.virtualScrollTimerId = _aspxSetTimeout("aspxGVVirtualScrollCore(\""+grid.name+"\");", 1000);
    },
    doScrollToRowIndex: function() {
        if(this.scrollToRowIndex < 0) return false;
        this.MakeRowVisible(this.scrollToRowIndex);
        this.scrollToRowIndex = -1;
        return true;
    },
    getVertScrollPosition: function() { return this.getScrollPosition(this.getScrollableControl(), true);  },
    getScrollPosition: function(scrollableDiv, isTop) {
        if(!_aspxIsExists(scrollableDiv)) return 0;
        return isTop ? scrollableDiv.scrollTop : scrollableDiv.scrollLeft;
    },
    setScrollPosition: function(scrollableDiv, newScrolPos, isTop) {
        if(newScrolPos <= 0) return;
        if(!_aspxIsExists(scrollableDiv)) return;
        this.lockVirtualScrolling = true;
        try {
            if(isTop) {
                scrollableDiv.scrollTop = newScrolPos;
            } else {
                scrollableDiv.scrollLeft = newScrolPos;
            }
        } finally {
            this.lockVirtualScrolling = false;
        }
    },
    getScrollableControl: function() {
        if(!this.isVerticalScrolling) return null;
        return this.GetMainTable().parentNode;
    },
    getHorzScrollableControl: function() {
        return this.isVerticalScrolling ? this.getScrollableControl() : this.GetHorzScrollDiv();
    },
    getScrollableHelper: function() {
        if(!this.isVerticalScrolling) return null;
        if(!_aspxIsExists(this.scrollableHelper)) {
            this.scrollableHelper = new ASPxClientTableScrollerHelper(this, "DXMainTable", "DXHeaderTable", "DXFooterTable", this.isHorizontalScrolling);
            if(_aspxIsExists(this.isWidthTypePercent)) {
                this.scrollableHelper.IsWidthTypePercent = true;
            }
        }
        return this.scrollableHelper;
    },
    VirtualScroll: function() {
        if(this.lockVirtualScrolling) return;
        if(this.getVertScrollPosition() != this.savedVirtualScroll) return;
        if(this.isVirtualScrollOutOfCurPage()){
            _aspxDetachEventFromElement(this.getScrollableControl(), "scroll", this.OnVirtualScroll); //this.getScrollableControl().onscroll = null;
            this.GotoPage(this.getVirtualPageIndex());
        }
    },
    isVirtualScrollOutOfCurPage: function() {
        var marginTop = this.getVirtualScrollMarginTop();
        if(this.savedVirtualScroll < marginTop) return true;
        if(this.pageRowCount < this.pageRowSize) return false;
        return this.savedVirtualScroll > marginTop + this.GetMainTable().clientHeight - this.getScrollableControl().clientHeight;
        
    },
    getVirtualPageIndex: function() {
        var marginTop = this.getVirtualScrollMarginTop();
        var rowIndex;
        if(this.savedVirtualScroll < marginTop) {
            rowIndex = this.savedVirtualScroll / this.virtualScrollRowHeight;
        } else {
            rowIndex = this.visibleStartIndex + this.pageRowCount + (this.savedVirtualScroll - marginTop) / this.virtualScrollRowHeight;
        }
        return Math.floor(rowIndex / this.pageRowSize);
    },
    getFixedColumnsHelper: function() {
        if(!_aspxIsExists(this.GetFixedColumnsDiv())) return null;
        if(!_aspxIsExists(this.fixedColumnsHelper)) {
            this.fixedColumnsHelper = new ASPxClientTableFixedColumnsHelper(this, 
            "DXMainTable", "DXHeaderTable", "DXFooterTable", "DXFixedColumnsDiv", "DXFixedColumnsContentDiv", 
            this.fixedColumnCount, this.GetRootTable().offsetWidth);
        }
        return this.fixedColumnsHelper;
    },
    UpdateVerticalScrollControls: function() {
        var helper = this.getScrollableHelper();
        if(helper == null) return;
        helper.SetOnScroll();
        helper.CorrectHeaderFooterColsWidth(false);
    },
    SetHeadersClientEvents: function() {
        var row = this.GetHeadersRow();
        if(row == null) return;
        for(var i = 0; i < row.cells.length; i ++) {
            if(this.isColumnsResizable) {
                _aspxAttachEventToElement(row.cells[i], "mousemove", new Function("event", "aspxGVHeaderColumnResizing('" + this.name + "', event);"));
            }
        }
    },
    UpdateHorzScrollDivs: function(onResize) {
		if(onResize && this.fixedColumnCount > 0) return;
		var divs = this.GetHorzScrollDivs();
		if(divs.length > 0) {
			var table = this.GetRootTable();
			if(ASPxClientGridView.IsOriginalWidthPercentage(table)) {				
				for(var i = 0; i < divs.length; i++)
					divs[i].style.width = "1px";
				ASPxClientGridView.ResetElementWidth(table);
			}
			var divWidth = (table.offsetWidth - ASPxClientGridView.GetBorders(table)) + "px";
			for(var i = 0; i < divs.length; i++)
				divs[i].style.width = divWidth;
		}
        if(this.getFixedColumnsHelper() != null) {
            this.getFixedColumnsHelper().Update();
        }
    },
    GetHorzScrollDivs: function() {
		var divs = [ this.GetHorzScrollDiv(), this.GetFixedColumnsDiv() ];
		var result = [ ];
		for(var i = 0; i < divs.length; i++) {
			if(!_aspxIsExistsElement(divs[i])) continue;
			result.push(divs[i]);
		}
		return result;
    },
    
    // API
    PerformCallback: function(args){
        if(!_aspxIsExists(args)) args = "";
        this.gridCallBack(["CUSTOMCALLBACK", args]);
    },
    GetValuesOnCustomCallback: function(args, onCallBack) {
        this.gridFuncCallBack(["CUSTOMVALUES", args], onCallBack);
    },
    GotoPage: function(pageIndex){
        this.gridCallBack(["GOTOPAGE", pageIndex]);
    },
    NextPage: function(){
        this.gridCallBack(["NEXTPAGE"]);
    },
    PrevPage: function(){
        this.gridCallBack(["PREVPAGE"]);
    },
    UnGroup: function(column){
        column = this._getColumnIndexByColumnArgs(column);
        this.GroupBy(column, -1);
    },
    ExpandAll: function(){
        this.gridCallBack(["EXPANDALL"]);
    },
    CollapseAll: function(){
        this.gridCallBack(["COLLAPSEALL"]);
    },
    ExpandAllDetailRows: function(){
        this.gridCallBack(["SHOWALLDETAIL"]);
    },
    CollapseAllDetailRows: function(){
        this.gridCallBack(["HIDEALLDETAIL"]);
    },
    ExpandRow: function(visibleIndex, recursive){
        if(this.RaiseRowExpanding(visibleIndex)) return;
        if(!_aspxIsExists(recursive)) recursive = false;
        this.gridCallBack(["EXPANDROW", visibleIndex, recursive]);
    },
    CollapseRow: function(visibleIndex, recursive){
        if(this.RaiseRowCollapsing(visibleIndex)) return;
        if(!_aspxIsExists(recursive)) recursive = false;
        this.gridCallBack(["COLLAPSEROW", visibleIndex, recursive]);
    },
    MakeRowVisible: function(visibleIndex) {
        var row = this.GetRow(visibleIndex);
        var helper = this.getScrollableHelper();
        if(helper == null || row == null) return;
        helper.MakeRowVisible(row);
    },
    ExpandDetailRow: function(visibleIndex){
        if(this.RaiseDetailRowExpanding(visibleIndex)) return;
        this.gridCallBack(["SHOWDETAILROW", visibleIndex]);
    },
    CollapseDetailRow: function(visibleIndex){
        if(this.RaiseDetailRowCollapsing(visibleIndex)) return;
        this.gridCallBack(["HIDEDETAILROW", visibleIndex]);
    },

    GetRowKey: function(visibleIndex) {
        var arrayIndex = visibleIndex - this.visibleStartIndex;
        if (arrayIndex < 0 || arrayIndex > this.keys.length - 1) return null;
        return this.keys[arrayIndex];
    },
    StartEditRow: function(visibleIndex) {
        var key = this.GetRowKey(visibleIndex);
        if(key != null)
            this.StartEditRowByKey(key);
    },
    StartEditRowByKey: function(key) {
        this.gridCallBack(["STARTEDIT", key]);
    },
    UpdateEdit: function(){
        this._updateEdit();
    },
    CancelEdit: function(){
        this.gridCallBack(["CANCELEDIT"]);
    },
    AddNewRow: function(){
        this.gridCallBack(["ADDNEWROW"]);
    },
    DeleteRow: function(visibleIndex){
        var key = this.GetRowKey(visibleIndex);
        if(key != null)
            this.DeleteRowByKey(key);
    },
    DeleteRowByKey: function(key) {
        this.gridCallBack(["DELETEROW", key]);
    },
    Refresh: function(){
        this.gridCallBack(["REFRESH"]);
    },
    ApplyFilter: function(expression){
        if(!_aspxIsExists(expression)) expression = "";
        this.gridCallBack(["APPLYFILTER", expression]);
    },
    ClearFilter: function(){
        this.ApplyFilter();
    },
    GetAutoFilterEditor: function(column) { 
        column = this._getColumnIndexByColumnArgs(column);
        if(!_aspxIsExists(column)) return null;
        return aspxGetControlCollection().Get(this.name + "_DXFREditorcol" + column);
    },
    AutoFilterByColumn: function(column,val){
        column = this._getColumnIndexByColumnArgs(column);
        if(!_aspxIsExists(column)) return;
        if(!_aspxIsExists(val)) val = "";
        if(val.length > 255) val = val.substr(0, 255);
        this.gridCallBack(["APPLYCOLUMNFILTER", column, val]);
    },
    ApplyHeaderFilterByColumn: function(column,val){
        column = this._getColumnIndexByColumnArgs(column);
        if(!_aspxIsExists(column)) return;
        if(!_aspxIsExists(val)) val = "";
        if(val.length > 500) val = val.substr(0, 500);
        this.gridCallBack(["APPLYHEADERCOLUMNFILTER", column, val]);
    },
    GroupBy: function(column, groupIndex, sortOrder){
        if(this.RaiseColumnGrouping(this._getColumnObjectByArg(column))) return;

        column = this._getColumnIndexByColumnArgs(column);
        if(!_aspxIsExists(groupIndex)) groupIndex = "";
        if(!_aspxIsExists(sortOrder)) sortOrder = "ASC";
        this.gridCallBack(["GROUP", column, groupIndex, sortOrder]);
    },
    SortBy: function(column, sortOrder, reset, sortIndex){
        if(this.RaiseColumnSorting(this._getColumnObjectByArg(column))) return;
        
        column = this._getColumnIndexByColumnArgs(column);
        if(!_aspxIsExists(sortIndex)) sortIndex = "";
        if(!_aspxIsExists(sortOrder)) sortOrder = "";
        if(!_aspxIsExists(reset)) reset = true;
        this.gridCallBack(["SORT", column, sortIndex, sortOrder, reset]);
    },
    MoveColumn: function(column, columnMoveTo, moveBefore, moveToGroup, moveFromGroup){
        if(!_aspxIsExists(column)) return;
        if(!_aspxIsExists(columnMoveTo)) columnMoveTo = -1;
        if(!_aspxIsExists(moveBefore)) moveBefore = true;
        if(!_aspxIsExists(moveToGroup)) moveToGroup = false;
        if(!_aspxIsExists(moveFromGroup)) moveFromGroup = false;
        if(moveToGroup) {
            if(this.RaiseColumnGrouping(this._getColumnObjectByArg(column))) return;
        }
        column = this._getColumnIndexByColumnArgs(column);
        this.gridCallBack(["COLUMNMOVE", column, columnMoveTo, moveBefore, moveToGroup, moveFromGroup]);
    },
    GetFocusedRowIndex: function() {
        return this._getFocusedRowIndex();
    },
    SetFocusedRowIndex: function(visibleIndex) {
        return this._setFocusedRowIndex(visibleIndex);
    },
    IsCustomizationWindowVisible: function(){
        var custWindow = this.GetCustomizationWindow();
        return custWindow != null && custWindow.IsVisible();
    },
    ShowCustomizationWindow: function(showAtElement){
        var custWindow = this.GetCustomizationWindow();
        if(!_aspxIsExists(showAtElement)) showAtElement = this.GetMainTable();
        custWindow.ShowAtElement(showAtElement);
    },
    HideCustomizationWindow: function(){
        var custWindow = this.GetCustomizationWindow();
        if(custWindow != null) custWindow.Hide();
    },
    SelectRows: function(visibleIndices, selected){
        if(!_aspxIsExists(selected)) selected = true;
        if(!_aspxIsExists(visibleIndices)) {
            selected = selected ? "all" : "unall";
            visibleIndices = [0];
        } else {
            if(visibleIndices.constructor != Array)
                visibleIndices = [visibleIndices];
        }
        this.gridCallBack(["SELECTROWS", selected].concat(visibleIndices));
    },
    SelectRowsByKey: function(keys, selected){
        if(!_aspxIsExists(selected)) selected = true;
        if(!_aspxIsExists(keys)) return;
        if(keys.constructor != Array)
            keys = [keys];
        this.gridCallBack(["SELECTROWSKEY", selected].concat(keys));
    },
    UnselectRowsByKey: function(keys){
        this.SelectRowsByKey(keys, false);
    },
    UnselectRows: function(visibleIndices){
        this.SelectRows(visibleIndices, false);
    },
    SelectRowOnPage: function(visibleIndex, selected){
        if(!_aspxIsExists(selected)) selected = true;
        this.SelectRow(visibleIndex, selected);
    },
    UnselectRowOnPage: function(visibleIndex){
        this.SelectRowOnPage(visibleIndex, false);
    },
    SelectAllRowsOnPage: function(selected){
        if(!_aspxIsExists(selected)) selected = true;
        this._selectAllRowsOnPage(selected);
    },
    UnselectAllRowsOnPage: function(){
        this._selectAllRowsOnPage(false);
    },
    GetSelectedRowCount: function() {
        return this._getSelectedRowCount();
    },
    GetSelectedFieldValues: function(fieldNames, onCallBack) {
        this.gridFuncCallBack(["SELFIELDVALUES", fieldNames], onCallBack);
    },
    GetRowValues: function(visibleIndex, fieldNames, onCallBack) {
        this.gridFuncCallBack(["ROWVALUES", visibleIndex, fieldNames], onCallBack);
    },
    GetPageRowValues: function(fieldNames, onCallBack) {
        this.gridFuncCallBack(["PAGEROWVALUES", fieldNames], onCallBack);
    },
    GetVisibleRowsOnPage: function() {
        return this.pageRowCount;
    },
    GetTopVisibleIndex: function() {
        return this.visibleStartIndex;
    },
    IsGroupRow: function(visibleIndex) {
        return this.GetGroupRow(visibleIndex) != null;
    },
    IsDataRow: function(visibleIndex) {
        return this.GetDataRow(visibleIndex) != null;
    },
    IsGroupRowExpanded: function(visibleIndex) { 
        return this.GetExpandedGroupRow(visibleIndex) != null;
    },
    GetColumnsCount: function() {
        return this._getColumnsCount();
    },
    GetColumn: function(index) {
        return this._getColumn(index);
    },
    GetColumnById: function(id) {
        return this._getColumnById(id);
    },
    GetColumnByField: function(fieldName) {
        return this._getColumnByField(fieldName);
    },
    GetEditor: function(column) {
        var columnObject = this._getColumnObjectByArg(column);
        return columnObject != null ? this.GetEditorByColumnIndex(columnObject.index) : null;
    },
    FocusEditor: function(column) {
        var editor = this.GetEditor(column);
        if(editor != null && _aspxIsExists(editor.SetFocus)) {
            editor.SetFocus();        
        }
    },
    GetEditValue: function(column) {
        var editor = this.GetEditor(column);
        return editor != null ? editor.GetValue() : null;
    },
    SetEditValue: function(column, value) {
        var editor = this.GetEditor(column);
        if(editor != null) {
            editor.SetValue(value);
        }
    },
    ShowFilterControl: function() {
        this.gridCallBack(["SHOWFILTERCONTROL"]);
    },
    CloseFilterControl: function() {
		this.HideFilterControlPopup();
        this.gridCallBack(["CLOSEFILTERCONTROL"]);
    },
    HideFilterControlPopup: function() {
		var popup = this.GetFilterControlPopup();
		if(popup) popup.Hide();
    },
    ApplyFilterControl: function() {
		var fc = this.GetFilterControl();
		if(fc == null) return;
		fc.Apply(this);
    },
    SetFilterEnabled: function(isFilterEnabled) {
        this.gridCallBack(["SETFILTERENABLED", isFilterEnabled]);
    },
    RaiseSelectionChanged: function(visibleIndex, isSelected, isAllRecordsOnPage, isChangedOnServer) {
        if(!this.SelectionChanged.IsEmpty()){
            var args = new ASPxClientGridViewSelectionEventArgs(visibleIndex, isSelected, isAllRecordsOnPage, isChangedOnServer);
            this.SelectionChanged.FireEvent(this, args);
            if(args.processOnServer) {
                this.gridCallBack(["SELECTION"]);
            }
        }
        return false; 
    },
    RaiseFocusedRowChanged: function() {
        if(!this.FocusedRowChanged.IsEmpty()){
            var args = new ASPxClientProcessingModeEventArgs(false);
            this.FocusedRowChanged.FireEvent(this, args);
            if(args.processOnServer) {
                this.gridCallBack(["FOCUSEDROW"]);
            }
        }
        return false; 
    },
    RaiseColumnSorting: function(column) {
        if(!this.ColumnSorting.IsEmpty()){
            var args = new ASPxClientGridViewColumnCancelEventArgs(column);
            this.ColumnSorting.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseColumnGrouping: function(column) {
        if(!this.ColumnGrouping.IsEmpty()){
            var args = new ASPxClientGridViewColumnCancelEventArgs(column);
            this.ColumnGrouping.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseColumnStartDragging: function(column) {
        if(!this.ColumnStartDragging.IsEmpty()){
            var args = new ASPxClientGridViewColumnCancelEventArgs(column);
            this.ColumnStartDragging.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseColumnResizing: function(column) {
        if(!this.ColumnResizing.IsEmpty()){
            var args = new ASPxClientGridViewColumnCancelEventArgs(column);
            this.ColumnResizing.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseRowExpanding: function(visibleIndex) {
        if(!this.RowExpanding.IsEmpty()){
            var args = new ASPxClientGridViewRowCancelEventArgs(visibleIndex);
            this.RowExpanding.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseRowCollapsing: function(visibleIndex) {
        if(!this.RowCollapsing.IsEmpty()){
            var args = new ASPxClientGridViewRowCancelEventArgs(visibleIndex);
            this.RowCollapsing.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseDetailRowExpanding: function(visibleIndex) {
        if(!this.DetailRowExpanding.IsEmpty()){
            var args = new ASPxClientGridViewRowCancelEventArgs(visibleIndex);
            this.DetailRowExpanding.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseDetailRowCollapsing: function(visibleIndex) {
        if(!this.DetailRowCollapsing.IsEmpty()){
            var args = new ASPxClientGridViewRowCancelEventArgs(visibleIndex);
            this.DetailRowCollapsing.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseRowClick: function(visibleIndex, htmlEvent) {
        if(!this.RowClick.IsEmpty()){
            var args = new ASPxClientGridViewRowClickEventArgs(visibleIndex, htmlEvent);
            this.RowClick.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseRowDblClick: function(visibleIndex, htmlEvent) {
        if(!this.RowDblClick.IsEmpty()){
            var args = new ASPxClientGridViewRowClickEventArgs(visibleIndex, htmlEvent);
            this.RowDblClick.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseContextMenu: function(objectType, index, htmlEvent) {
        if(!this.ContextMenu.IsEmpty()){
            var args = new ASPxClientGridViewContextMenuEventArgs(objectType, index, htmlEvent);
            this.ContextMenu.FireEvent(this, args);
            return true;
        }
        return false; 
    },
    RaiseCustomizationWindowCloseUp: function() {
        if(!this.CustomizationWindowCloseUp.IsEmpty()){
            var args = new ASPxClientEventArgs();
            this.CustomizationWindowCloseUp.FireEvent(this, args);
        }
        return false; 
    },
    RaiseColumnMoving: function(targets) {
        if(this.ColumnMoving.IsEmpty()) return;
        var srcColumn = this.getColumnObject(targets.obj.id);
        var destColumn = this.getColumnObject(targets.targetElement.id);
        var isLeft = targets.isLeftPartOfElement();
        var isGroupPanel = this.IsTagertElementGroupPanel(targets);
        var args = new ASPxClientGridViewColumnMovingEventArgs(srcColumn, destColumn, isLeft, isGroupPanel);
        this.ColumnMoving.FireEvent(this, args);
        if(!args.allow) {
            targets.targetElement = null;
        }
    }
});
ASPxClientGridViewColumn = _aspxCreateClass(null, {
	constructor: function(name, index, fieldName, visible){
        this.name = name;
        this.id = name;
        this.index = index;
        this.fieldName = fieldName;
        this.visible = visible;
	}
});
ASPxClientGridViewColumnCancelEventArgs = _aspxCreateClass(ASPxClientCancelEventArgs, {
	constructor: function(column){
	    this.constructor.prototype.constructor.call(this, true);
        this.column = column;
    }
});
ASPxClientGridViewRowCancelEventArgs = _aspxCreateClass(ASPxClientCancelEventArgs, {
	constructor: function(visibleIndex){
	    this.constructor.prototype.constructor.call(this, true);
        this.visibleIndex = visibleIndex;
    }
});
ASPxClientGridViewSelectionEventArgs = _aspxCreateClass(ASPxClientProcessingModeEventArgs, {
	constructor: function(visibleIndex, isSelected, isAllRecordsOnPage, isChangedOnServer){
	    this.constructor.prototype.constructor.call(this, false);
        this.visibleIndex = visibleIndex;
        this.isSelected = isSelected;
        this.isAllRecordsOnPage = isAllRecordsOnPage;
        this.isChangedOnServer = isChangedOnServer;
    }
});
ASPxClientGridViewRowClickEventArgs = _aspxCreateClass(ASPxClientGridViewRowCancelEventArgs, {
	constructor: function(visibleIndex, htmlEvent){
	    this.constructor.prototype.constructor.call(this, visibleIndex);
        this.htmlEvent = htmlEvent;
    }
});
ASPxClientGridViewContextMenuEventArgs = _aspxCreateClass(ASPxClientEventArgs, {
	constructor: function(objectType, index, htmlEvent){
	    this.constructor.prototype.constructor.call(this);
        this.objectType = objectType;
        this.index = index;
        this.htmlEvent = htmlEvent;
    }
});
ASPxClientGridViewCustomButtonEventArgs = _aspxCreateClass(ASPxClientProcessingModeEventArgs, {
	constructor: function(visibleIndex, buttonID) {
		this.constructor.prototype.constructor.call(this, false);
		this.visibleIndex = visibleIndex;
		this.buttonID = buttonID;
	}	
});
ASPxClientGridViewColumnMovingEventArgs = _aspxCreateClass(ASPxClientEventArgs, {
	constructor: function(sourceColumn, destinationColumn, isDropBefore, isGroupPanel) {
		this.constructor.prototype.constructor.call(this);
		this.allow = true;
		this.sourceColumn = sourceColumn;
		this.destinationColumn = destinationColumn;
		this.isDropBefore = isDropBefore;
		this.isGroupPanel = isGroupPanel;
	}	
});



//TODO move to classes
///////////////////////////////////////////////////////////////////////////////////
//function _aspxGetParentByPartialId(element, idPart){
//	element = _aspxGetParentNode(element);
//	while(element != null){
//	    if(_aspxIsExists(element.id)) {
//		    if(element.id.indexOf(idPart) > -1) return element;
//		}
//		element = _aspxGetParentNode(element);
//	}
//	return null;
//}

function aspxGetCtrlKey(evt) { 
	if(__aspxIE) return (event != null) ? event.ctrlKey : false;
	else return (evt != null) ? evt.ctrlKey : false;
}
function aspxGetAltKey(evt) { 
	if(__aspxIE) return (event != null) ? event.altKey : false;
	else return (evt != null) ? evt.altKey : false;
}

function aspxGetShiftKey(evt) { 
	if(__aspxIE) return (event != null) ? event.shiftKey : false;
	else return (evt != null) ? evt.shiftKey : false;
}

function _aspxGetIECaretInfo(element, operation){
	if(!_aspxIsExists(document.selection) || !_aspxIsExists(document.selection.createRange)) 
	    return -1;
	var origionalRange = document.selection.createRange();
	try {
		var range = origionalRange.duplicate();
		range.moveToElementText(element);
	} 
	catch(e){
	    try {
		    var range = element.createTextRange();
		}
		catch(ee) {
		    return -1;
		}
	}
	range.setEndPoint(operation, origionalRange);
	var result = range.text.length;
	return result > element.value.length ? -1 : result;
}

function _aspxGetCaretEnd(element){
    try {
	    if(_aspxIsExists(element.selectionEnd))
		    return element.selectionEnd;
	    return _aspxGetIECaretInfo(element, "EndToEnd");
	}
	catch(e) {
	    return null;
	}
}
function _aspxCaretStart(element){
	if(_aspxIsExists(element.selectionStart)) 
	    return element.selectionStart;
	return _aspxGetIECaretInfo(element, "EndToStart");
}

function _aspxSetCaret(element, pos){
	element.focus();
    if(_aspxIsExists(element.type) && element.type != "text") return;
	if(_aspxIsExists(element.setSelectionRange)){
		element.setSelectionRange(pos, pos);
		return;
	}
	if(_aspxIsExists(element.createTextRange)){
		range = element.createTextRange();		
		range.moveStart('character',pos);
		range.collapse();
		range.select();
	}
}

////////////////////////////////////////////////////////////////////
function aspxGVContextMenu(name, objectType, index, e) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        gv.RaiseContextMenu(objectType, index, e);
    }
}
function aspxGVExpandRow(name, value) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.ExpandRow(value);
}
function aspxGVCollapseRow(name, value) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.CollapseRow(value);
}
function aspxGVShowDetailRow(name, value) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.ExpandDetailRow(value);
}
function aspxGVHideDetailRow(name, value) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.CollapseDetailRow(value);
}
function aspxGVStartEditRow(name, value) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.StartEditRow(value);
}
function aspxGVDeleteRow(name, value) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.DeleteGridRow(value);
}
function aspxGVClearFilter(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.ClearFilter();
}
function aspxGVUpdateEdit(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.UpdateEdit();
}
function aspxGVCancelEdit(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.CancelEdit();
}
function aspxGVAddNewRow(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.AddNewRow();
}
function aspxGVSelectRow(name, index, value) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.SelectRowCore(index, value);
}
function aspxGVCommandCustomButton(name, id, index) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.CommandCustomButton(id, index);
}
function aspxGVHeaderMouseDown(name, element, e) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.HeaderMouseDown(element, e);
}
function aspxGVHeaderColumnResizing(name, e) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.HeaderColumnResizing(e);
}

function aspxGVPagerOnClick(name, value) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.doPagerOnClick(value);
}
function aspxGVFilterKeyPress(name, element, e) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.OnColumnFilterInputKeyPress(element, e);
}
function aspxGVFilterSpecKeyPress(name, element, e) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.OnColumnFilterInputSpecKeyPress(element, e);
}
function aspxGVFilterChanged(name, element) {
    _aspxSetTimeout("aspxGVFilterChangedDelayed(\""+name+"\", \"" + element.name + "\");", 0);
}
function aspxGVFilterChangedDelayed(name, elementName) {
    var gv = aspxGetControlCollection().Get(name);
    var element = aspxGetControlCollection().Get(elementName);
    if(gv != null && element != null) 
        gv.OnColumnFilterInputChanged(element);
}
function aspxGVTimer(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.OnFilterKeyPressTick();
}
function aspxGVFocusedRowChanged(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.RaiseFocusedRowChanged();
}
function aspxGVSelectionChanged(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.RaiseSelectionChanged(-1, false, false, true);
}
function aspxGVFuncCallback(name, result) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.OnFuncCallback(unescape(result));
}

function aspxRestoreCallBackTimer(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) 
        gv.OnAfterCallback();
}
function aspxGVShowParentRows(name, evt, element) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        if(_aspxIsExists(element)) {
            gv.OnParentRowMouseEnter(element);
        }
        else gv.OnParentRowMouseLeave(evt);
    }
}
function aspxGVTableClick(name, evt) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        gv.mainTableClick(evt);
    }
    return _aspxCancelBubble(evt); // B131024
}
function aspxGVTableDblClick(name, evt) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        gv.mainTableDblClick(evt);
    }
    return _aspxCancelBubble(evt); // B132161
}
function aspxGVParentRowsTimer(name, rowId) {
    var gv = aspxGetControlCollection().Get(name);
    var element = _aspxGetElementById(rowId);
    if(!_aspxIsExists(element) || gv == null) return;
    gv.ShowParentRows(element);
}

function aspxGVCustWindowCloseUp(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        gv.RaiseCustomizationWindowCloseUp();
    }
}

function aspxGVShowFilterPopup(name, elementName, colIndex, buttonElement, htmlEvent) {	
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
		buttonElement.DXFilterPopupButton = 1;
		gv.ShowFilterPopup(elementName, colIndex, htmlEvent);
    }
}
function aspxGVApplyFilterPopup(name, colIndex, row) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        gv.HideFilterPopup();
        gv.ApplyHeaderFilterByColumn(colIndex, _aspxGetAttribute(row, "filterValue"));
    }
}
function aspxGVFilterPopupItemOver(name, row) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        gv.onFilterItemOver(row);
    }
}
function aspxGVFilterPopupItemOut(name, row) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        gv.onFilterItemOut(row);
    }
}
function aspxGVShowFilterControl(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        gv.ShowFilterControl();
    }
}
function aspxGVCloseFilterControl(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        gv.CloseFilterControl();
    }
}
function aspxGVSetFilterEnabled(name, value) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null) {
        gv.SetFilterEnabled(value);
    }
}
function aspxGVApplyFilterControl(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null)
        gv.ApplyFilterControl();
}

function aspxGVFilterRowMenu(name, columnIndex, element) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null)
        gv.FilterRowMenuButtonClick(columnIndex, element);
}
function aspxGVFilterRowMenuClick(name, e) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null)
		gv.FilterRowMenuItemClick(e.item);
}
function aspxGVSort(name, columnIndex) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null)		
		gv.SortBy(columnIndex);
}

function aspxGVVirtualScrollCore(name) {
    var gv = aspxGetControlCollection().Get(name);
    if(gv != null)		
		gv.VirtualScroll();
}

// TODO: move to Classes.js
function _aspxSerializeStringArray(array) {
    if(!_aspxIsExists(array) || array.constructor != Array || array.length == 0)
        return "";
    var sb = new ASPxStringBuilder();
    for(var i = 0; i < array.length; i++) {
        var item = array[i].toString();
        sb.Append(item.length);
        sb.Append('|');
        sb.Append(item);
    }
    return sb.ToString();
}

ASPxClientGridView.instances = { };
ASPxClientGridView.ResetElementWidth = function(element) {
	var width;
	if(!_aspxIsExists(element.dxgvOrigWidth)) {		
		width = element.dxgvOrigWidth = String(element.style.width).length > 0
			? element.style.width 
			: element.offsetWidth + "px";
	} else {
		 width = element.dxgvOrigWidth;
	}	
	element.style.width = width;
};
ASPxClientGridView.IsOriginalWidthPercentage = function(element) {
	var width = String(element.style.width);
	return width.length < 1 || width.charAt(width.length - 1) == "%";
};
ASPxClientGridView.GetBorders = function(element) {
    var curStyle = _aspxGetCurrentStyle(element);
    var res = _aspxPxToInt(curStyle.paddingLeft) + _aspxPxToInt(curStyle.paddingRight);
    if(curStyle.borderLeftStyle != "none")
		res += _aspxPxToInt(curStyle.borderLeftWidth);
    if(curStyle.borderRightStyle != "none")
		res += _aspxPxToInt(curStyle.borderRightWidth);        
    return res;
};