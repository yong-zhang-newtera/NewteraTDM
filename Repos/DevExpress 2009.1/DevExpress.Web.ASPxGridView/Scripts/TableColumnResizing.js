var __aspxTableColumnResizing = null;
var __aspxScrollerHelpers = null;

ASPxClientTableHelper = _aspxCreateClass(null, {
	constructor: function(control, contentTableName, headerTableName, footerTableName, isHorizontalScrolling){
	    this.Control = control;
	    this.ContentTableName = contentTableName;
	    this.HeaderTableName = headerTableName;
	    this.FooterTableName = footerTableName;
	    this.IsHorizontalScrolling = isHorizontalScrolling;
	    this.groupCols = null;
    },
    GetContentTable: function() { return this.Control.GetChildElementById(this.ContentTableName); },
    GetHeaderTable: function() { return this.Control.GetChildElementById(this.HeaderTableName); },
    GetFooterTable: function() { return this.Control.GetChildElementById(this.FooterTableName); },
    GetResizableTable: function() { return this.GetHeaderTable() != null ?  this.GetHeaderTable() : this.GetContentTable(); },
    getScrollBarWidth: function(table) {
        if(table.offsetParent.offsetHeight > table.offsetHeight) return 0;
        return this.getScrollBarWidthCore();
    },
    getScrollBarWidthCore: function() {
        if(this.scrollBarWidth > 0) return this.scrollBarWidth;
        var inner = document.createElement("p");
        inner.style.cssText = "width: 100%; height: 200px;";
        var outer = document.createElement("div");
        outer.appendChild(inner);
        outer.style.cssText = "position: absolute; visibility: hidden; width: 200px; height: 150px; overflow: hidden";
        document.body.appendChild(outer);
        var w1 = inner.offsetWidth;
        outer.style.overflow = "scroll";
        var w2 = inner.offsetWidth;
        if (w1 == w2) w2 = outer.clientWidth;                                 
        this.scrollBarWidth = (w1 - w2);
        document.body.removeChild(outer);
        return this.scrollBarWidth;
    },
    GetTableLastColGroup: function(table) {
        if(table == null) return null;
        var res = _aspxGetElementsByTagName(table, "COLGROUP");
        var res = res.length > 0 ? res[res.length - 1] : null;
        return res != null ? _aspxGetElementsByTagName(res, "COL") : null;
    },
    ChangeCellStyleWidth: function(cell, newColWidth) {
        if(_aspxIsExists(cell) && _aspxIsExists(cell.style)) {
            cell.style.width = newColWidth;   
        }
    },
    getWidthFromStyle: function(width) {
        if(!_aspxIsExists(width) || width == "" || width.indexOf("%") > -1) return -1;
        return width.indexOf("px") > -1 ? _aspxPxToInt(width) : parseInt(width);
    },
    getElementStyleOrigionalWidth: function(element) {
        var col = this._getColByCell(element, this.groupCols);
        var res = -1;
        if(col != null) {
            res = this.getWidthFromStyle(col.width);
        } else {
            if(_aspxIsExists(element.style)) {
                res = this.getWidthFromStyle(element.style.width);
            }
        }
        if(res > 0) return res;
        return element.offsetWidth - ASPxClientGridView.GetBorders(element);
    },
    _getColByCell: function(column, groupCols) {
        if(groupCols == null) return null;
        return groupCols[column.cellIndex];
    }
});

ASPxClientTableColumnResizing = _aspxCreateClass(ASPxClientTableHelper, {
	constructor: function(control, contentTableName, headerTableName, footerTableName, isHorizontalScrolling){
		this.constructor.prototype.constructor.call(this, control, contentTableName, headerTableName, footerTableName, isHorizontalScrolling);
	    this.colIndex = -1;
        this.contentWidthRow = -2;
	    this.column = null;
	    this.scrollerHelper = null;
	    this.maximumOffset = 3;
	    this.minimumWidth = 6;
	    this.prevX = 0;
	    this.resizedInput = null;
	    this.resizableControl = null;
	    this.editorsToBeResized = null;
	    this.CanResizeLastColumn = true;
    },
    StartResizing: function(column, evt, indentColumnCount) {    
        this.groupCols = this.GetTableLastColGroup(this.GetResizableTable());
        if(_aspxGetEventX(evt) - _aspxGetAbsoluteX(column) < this.maximumOffset)
			column = column.parentNode.cells[column.cellIndex - 1];
        this.column = column;
        this.colIndex = this._getColByCell(column, this.groupCols);
        __aspxTableColumnResizing = this;
        if(this.GetHeaderTable() != null) {
            this.scrollerHelper = new ASPxClientTableScrollerHelper(this.Control, this.ContentTableName, this.HeaderTableName, this.FooterTableName, this.IsHorizontalScrolling);
        }
    },
    CanStartResizing: function(column, evt, indentColumnCount) {
        var table = this.GetResizableTable();
        if(!_aspxIsExists(table)) return false;
        var columnType = this._getTableCellType(table, column, indentColumnCount);
        if(columnType < 0) return false;
        this.prevX = _aspxGetEventX(evt);
        var left = _aspxGetAbsoluteX(column);
        var right = left + column.offsetWidth - 1;        
        if(this.prevX - left < this.maximumOffset)
			return column.cellIndex > indentColumnCount;
		if(right - this.prevX <= this.maximumOffset)
			return this.CanResizeLastColumn || columnType != 1;
		return false;
    },
    SetResizedInput: function(input) {
        this.resizedInput = input;
    },
    IsResizing: function() { return __aspxTableColumnResizing == this; },
	UpdateCursor: function(element, evt, indentColumnCount) {
		var table = this.GetResizableTable();
		var level = 0;
		var temp = element;
		while(temp != null && temp != table) {
			temp = temp.parentNode;
			level++;
		}
		if(temp == null) return;
		while(level > 3) {
			element = element.parentNode;
			level--;
		}
		var key = "__DXPrevCursor";		
		if(this.IsResizing() || this.CanStartResizing(element, evt, indentColumnCount)) {
			_aspxChangeStyleAttribute(element, "cursor", "w-resize");
		} else {
			_aspxResetStyleAttribute(element, "cursor");
		}
	},
    SetResizableControl: function(resizableControl) {
        this.resizableControl = resizableControl;
    },
    SetEditorsToBeResized: function(list) {
        this.editorsToBeResized = list;
    },
    onmouseup: function(evt) {
        this.endResizing();
        this.CancelResizing(evt);
    },
    onmousemove: function(evt) {
		if(this.column.cellIndex < 0) {
			// B134111
			this.CancelResizing(evt);
			return;
		}
        var newX = _aspxGetEventX(evt);
        var delta = newX - this.prevX;
        var newColWidth = this.getElementStyleOrigionalWidth(this.column) + delta;
        if(newColWidth < this.minimumWidth) return;
        if(!this.canApplyResizableElementsWidth(-delta)) return;
        if(this._changeColumnWidth(this.column, newColWidth)) {
            this.applyResizableElementsWidth(-delta);
            this.prevX = newX;
        }
    },
    CancelResizing: function(evt) {
        __aspxTableColumnResizing = null;
        this.contentWidthRow = -2;
	    this.groupCols = null;
	    this.column = null;
	    this.prevX = 0;
	    this.resizedInput = null;
	    this.resizableControl = null;
	    this.editorsToBeResized = null;
	    if(this.scrollerHelper != null) {
	        this.scrollerHelper.CorrectHeaderFooterColsWidth();
	    }
    },
    canApplyResizableElementsWidth: function(delta) {
        if(_aspxIsExists(this.resizableControl)) return true;
        var changedColumn = this._getNextChangedColumn(this.column, delta);
        if(changedColumn == null) return false;
        var oldWidth = this.getElementStyleOrigionalWidth(changedColumn);
        return oldWidth + delta >= this.minimumWidth;
    },
    applyResizableElementsWidth: function(delta) {
        if(_aspxIsExists(this.resizableControl)) {
            if(this.IsHorizontalScrolling) return;
            var curWidth = this.getElementStyleOrigionalWidth(this.resizableControl);
            this.resizableControl.style.width = (curWidth - delta) + "px";
            if(this.scrollerHelper != null) {
                this.scrollerHelper.SetHeaderFooterContainersWidth();
            }
        } 
        else {
            var changedColumn = this._getNextChangedColumn(this.column, delta);
            if(changedColumn == null) return;
            var oldWidth = this.getElementStyleOrigionalWidth(changedColumn);
            this._changeColumnWidth(changedColumn, oldWidth + delta);
        }
    },
    endResizing: function() {
        if(!_aspxIsExists(this.resizedInput) || !_aspxIsExists(this.column)) return;
        var cells = this.column.parentNode.cells;
        var res = "";
        for(var i = 0; i < cells.length; i ++) {
            width = this.getElementStyleOrigionalWidth(cells[i]);
            res += width + "|";
        }
        this.resizedInput.value = res;
        this.CorrectEditorsBounds();
    },
    CorrectEditorsBounds: function() {
        if(this.editorsToBeResized == null) return;
        for(var i = 0; i < this.editorsToBeResized.length; i ++)
            this.editorsToBeResized[i].AdjustControl(false);
    },
    _changeColumnWidth: function(column, newColWidth){
        var oldColWidth = this.getElementStyleOrigionalWidth(column);
        this._changeColumnWidthCore(this.GetResizableTable(), newColWidth, this.groupCols, column.cellIndex, 0);
        var result = oldColWidth != this.getElementStyleOrigionalWidth(column);
        if(result && this.GetHeaderTable() != null) {
            this._changeColumnWidthAnotherTable(this.GetContentTable(), column.cellIndex, newColWidth, this.GetContentWidthRow());
            this._changeColumnWidthAnotherTable(this.GetFooterTable(), column.cellIndex, newColWidth, 0);
        }
        return result;
    },
    _changeColumnWidthAnotherTable: function(table, columnIndex, newColWidth, rowIndex){
        if(table == null) return;
        this._changeColumnWidthCore(table, newColWidth, this.GetTableLastColGroup(table), columnIndex, rowIndex);
    },
    _changeColumnWidthCore: function(table, newColWidth, groupCols, colIndex, rowIndex){
        if(newColWidth < 0) return;
        var col =  groupCols[colIndex];
        col.width = newColWidth + "px";
        if(rowIndex > -1) {
            var row = table.rows[rowIndex];
            row.cells[colIndex].style.width = newColWidth + "px";
        }
    },
    _getNextChangedColumn: function(column, dWidth) {
        if(column.cellIndex == 0 && column.parentNode.cells.length == 1) return null;
        var cellIndex = column.cellIndex + 1 < column.parentNode.cells.length ? column.cellIndex + 1 : column.cellIndex - 1;
        return column.parentNode.cells[cellIndex];
    },
    GetContentWidthRow: function() {
        if(this.contentWidthRow > -2) return this.contentWidthRow;
        this.contentWidthRow = -1;
        var colCount = this.groupCols.length;
        var table = this.GetContentTable();
        for(var i = 0; i < table.rows.length; i ++) {
            if(table.rows[i].cells.length == colCount) {
                this.contentWidthRow = i;
                break;
            }
        }
        return this.contentWidthRow;
    },
    //-1 - is not exists, 0 - exists, 1 - exists and it is last
    _getTableCellType: function(table, column, indentColumnCount) {
        for(var i = 0; i < table.rows.length; i ++) {
            var row = table.rows[i];
            for(var j = 0; j < row.cells.length; j ++) {
                if(row.cells[j] == column) {
                    if(column.cellIndex < indentColumnCount) return -1;
                    return j == row.cells.length - 1 ? 1 : 0;
                }
            }
        }
        return -1;
    }
});

ASPxClientTableScrollerHelper = _aspxCreateClass(ASPxClientTableHelper, {
	constructor: function(control, contentTableName, headerTableName, footerTableName, isHorizontalScrolling){
		this.constructor.prototype.constructor.call(this, control, contentTableName, headerTableName, footerTableName, isHorizontalScrolling);
	    this.scrollBarWidth = -1;
	    this.IsWidthTypePercent = false;
	    this.isScrolling = false;
	    this.SetOnScroll();
	    if(__aspxScrollerHelpers == null) {
	        __aspxScrollerHelpers = new Array();
	    }
	    __aspxScrollerHelpers.push(this);
    },
    CorrectHeaderFooterColsWidth : function() {
        this.SetHeaderFooterContainersWidth();
    },
    AfterWindowResize: function() {
        this.CorrectHeaderFooterColsWidth();
    },
    OnWindowResize: function(index) {
        if(!this.IsWidthTypePercent) return;
        _aspxSetTimeout("TableScrollerHelper_AfterWindowResize(" + index + ");", 0);
    },
    SetOnScroll: function() {
        if(!this.IsHorizontalScrolling) return;
        this.SetOnScrollCore(this.GetContentTable(), this.OnContentScroll);
        this.SetOnScrollCore(this.GetHeaderTable(), this.OnHeaderScroll);
    },
    SetOnScrollCore: function(table, proc) {
        var parent = this.GetScrollDiv(table);
        if(parent != null) {
            _aspxAttachEventToElement(parent, "scroll", proc); //parent.onscroll = proc;
            parent.scrollHelper = this;
        }
    },
    OnContentScroll: function(event) {
        var src = _aspxGetEventSource(event);
        var scrollHelper = src.scrollHelper;
        if(!_aspxIsExists(scrollHelper)) return;
        scrollHelper.OnDivScrollCore(src, scrollHelper.GetHeaderTable(), scrollHelper.GetFooterTable());
    },
    OnHeaderScroll: function(event) {
        var src = _aspxGetEventSource(event);
        var scrollHelper = src.scrollHelper;
        if(!_aspxIsExists(scrollHelper)) return;
        scrollHelper.OnDivScrollCore(src, scrollHelper.GetContentTable(), scrollHelper.GetFooterTable());
    },
    OnDivScrollCore: function(src, table1, table2) {
        if(this.isScrolling) return;
        this.isScrolling = true;
        this.SetScrollLeft(src, table1);
        this.SetScrollLeft(src, table2);
        this.isScrolling = false;
    },
    GetScrollDiv: function(table) {
        if(!_aspxIsExists(table)) return null;
        return table.parentNode;
    },
    SetScrollLeft: function(src, table) {
        var div = this.GetScrollDiv(table);
        if(div != null) {
            div.scrollLeft = src.scrollLeft;
        }
    },
    GetColsWidth: function(table) {
        if(table.rows.length < 2) return null;
        var widths = new Array();
        var row = table.rows[table.rows.length - 1];
        for(var i = 0; i < row.cells.length; i ++) {
            var width = row.cells[i].clientWidth;
            if(__aspxNS && (i == row.cells.length - 1)) {
                width += this.getScrollBarWidth(table);
            }
            widths.push(width);
        }
        return widths;
    },
    SetHeaderFooterContainersWidth: function() {
		if(!this.Control.IsVisible()) {
			this.Control.sizeCorrectedOnce = false;
			return;
		}
		var mainEl = this.Control.GetMainElement();
		if(!mainEl) return;
		
		var headerTable = this.GetHeaderTable();
		this.ResetContainerWidth(headerTable);
	    this.ResetContainerWidth(this.GetContentTable());
	    this.ResetContainerWidth(this.GetFooterTable());
    
		ASPxClientGridView.ResetElementWidth(mainEl);
		
		var mainElWidth = mainEl.offsetWidth;
        if(!this.IsHorizontalScrolling && headerTable && (__aspxOpera || __aspxFirefox)) {
            var headerWidth = headerTable.offsetWidth;
            if(headerWidth > mainElWidth)
                mainElWidth = 2 + headerWidth + this.getScrollBarWidthCore();
        }		
		var width = mainElWidth - ASPxClientGridView.GetBorders(mainEl) - ASPxClientGridView.GetBorders(mainEl.rows[0].cells[0]);
		
        this.SetContainerWidth(headerTable, width, this.getScrollBarWidthCore());
        this.SetContainerWidth(this.GetContentTable(), width, 0);
        this.SetContainerWidth(this.GetFooterTable(), width, this.getScrollBarWidthCore());
        // B134537, B135041
        if(!this.IsHorizontalScrolling || this.Control.fixedColumnCount < 1) {
		    this.SetElementWidth(this.GetContentTable(), width, this.getScrollBarWidthCore(), 0);
		}        
        
        mainEl.style.width = mainElWidth + "px";
    },
    SetContainerWidth: function(table, width, margin) {
        if(table == null) return;
        var parent = table.parentNode;
        if(!_aspxIsExists(parent)) return;
        parent.align = "left";
        this.SetElementWidth(parent, width, margin, margin);
    },
    ResetContainerWidth: function(table) {
		if(table == null) return;
		var parent = table.parentNode;
		if(!_aspxIsExists(parent)) return;
		parent.style.width = "1px";
    },
    SetElementWidth: function(element, width, margin, marginRight) {
        if(!_aspxIsExists(element)) return;
        if(width <= this.getScrollBarWidthCore()) return;
        if(marginRight > 0) {
            if(_aspxIsElementRigthToLeft(element) && !(__aspxOpera || __aspxSafariFamily)) {
                element.style.marginLeft = marginRight + "px";
                element.style.marginRight = "";
            }
            else {
            element.style.marginRight = marginRight + "px";
                element.style.marginLeft = "";
            }
        }
        element.style.width = (width - margin) + "px";
    },
    MakeRowVisible: function(row) {
        var div = this.GetScrollDiv(this.GetContentTable());
        if(div == null) return;
        var top = row.offsetTop;
        if(top + row.offsetHeight < div.offsetHeight) {
            top = 0;
        }
        div.scrollTop = top;
    }
});
ASPxClientTableScrollerHelper.prevBodyWidth = -1;

ASPxClientTableFixedColumnsHelper = _aspxCreateClass(ASPxClientTableHelper, {
	constructor: function(control, contentTableName, headerTableName, footerTableName, fixedDivName, fixedContentDivName, fixedColumnCount, areaWidth){
		this.constructor.prototype.constructor.call(this, control, contentTableName, headerTableName, footerTableName, true);
		this.FixedDivName = fixedDivName;
		this.FixedContentDivName = fixedContentDivName;
		this.FixedColumnCount = fixedColumnCount; 
		this.hiddenColumnCount = 0;
		this.prevHiddenColumnCount = 0;
		this.doScrolling = false;
		this.areaWidth = areaWidth;
		this.Update();
    },
    GetFixedDiv: function() { return this.Control.GetChildElementById(this.FixedDivName); },
    GetFixedContentDiv: function() { return this.Control.GetChildElementById(this.FixedContentDivName); },
    Update: function() {
        _aspxDetachEventFromElement(this.GetFixedDiv(), "scroll", this.OnScroll);//this.GetFixedDiv().onscroll = null;
        this.GetFixedDiv().helper = this;
        this.UpdateRowsHeight();
        this.groupCols = this.GetTableLastColGroup(this.GetResizableTable());
        this.scrollableWidth = this.GetScrollableWidth();
        this.GetFixedContentDiv().style.width = this.GetContentWidth() + "px";
        var scrollHeigth = this.getScrollBarWidthCore() + "px";
        this.GetFixedDiv().style.height = scrollHeigth;
        this.GetFixedContentDiv().style.height = scrollHeigth;
        _aspxAttachEventToElement(this.GetFixedDiv(), "scroll", this.OnScroll); //this.GetFixedDiv().onscroll = this.OnScroll;
    },
    OnScroll: function(event) {
        var src = _aspxGetEventSource(event);
        if(src.doScrolling) return;
        src.doScrolling = true;
        var helper = src.helper;
        var columnsToHide = helper.GetColumnsToHide(src.scrollLeft);
        if(helper.hiddenColumnCount != columnsToHide) {
            helper.prevHiddenColumnCount = helper.hiddenColumnCount;
            helper.hiddenColumnCount = columnsToHide;
            helper.UpdateCells();
        }
        src.doScrolling = false;
    },
    GetContentWidth: function() {
        return this.GetResizableTable().offsetWidth + this.getScrollBarWidthCore();
    },
    GetScrollableWidth: function() {
        var res = this.GetContentWidth();
        var tr = this.GetHeaderRow();
        for(i = 0;  i < this.FixedColumnCount; i ++) {
            res -= this.GetColumnWidth(tr, i);
        }
        return res;
    },
    GetHeaderRow: function() {
        return this.GetResizableTable().rows[0];        
    },
    GetColumnsToHide: function(scrollLeft) {
        var width = 0;
        var tr = this.GetHeaderRow();
        var scrollWidth = this.GetFixedContentDiv().scrollWidth + this.areaWidth;
        var i;
        for(i = this.FixedColumnCount; i < tr.cells.length; i ++) {
            if(scrollLeft * scrollWidth <= width * this.scrollableWidth) break;
            width += this.GetColumnWidth(tr, i);
        }
        if(i == tr.cells.length) i --;
        return i - this.FixedColumnCount;
    },
    GetColumnWidth: function(tr, index) {
        return this.groupCols != null ? parseInt(this.groupCols[index].width) : this.getElementStyleOrigionalWidth(tr.cells[index])
    },
    UpdateCells: function() {
        this.UpdateTableCells(this.GetHeaderTable());
        this.UpdateTableCells(this.GetContentTable());
        this.UpdateTableCells(this.GetFooterTable());
    },
    UpdateTableCells: function(table) {
        if(!_aspxIsExists(table)) return;
        var startIndex = Math.min(this.prevHiddenColumnCount, this.hiddenColumnCount) + this.FixedColumnCount;
        var endIndex = Math.max(this.prevHiddenColumnCount, this.hiddenColumnCount) + this.FixedColumnCount;
        var display = this.hiddenColumnCount - this.prevHiddenColumnCount > 0 ? "none" : "";
        var cols = this.GetTableLastColGroup(table);
        if(cols != null) {
            for(var i = startIndex; i < endIndex; i ++) {
                cols[i].style.display = display;
            }
        } 
        if(cols == null || !__aspxIE){       
            for(var i = 0; i < table.rows.length; i ++) {
                this.UpdateRowCells(table.rows[i], startIndex, endIndex, display);
            }
        }
    },
    UpdateRowCells: function(row, startIndex, endIndex, display) {
        for(var i = startIndex; i < endIndex; i ++) {
            row.cells[i].style.display = display;
        }
    },
    UpdateRowsHeight: function() {
        var table = this.GetContentTable();
        for(var i = 0; i < table.rows.length; i ++) {
            var row = table.rows[i];
            if(row.cells.length == 0) continue;
            var cell = row.cells[0];
            var height = _aspxGetClearClientHeight(cell) + "px";
            row.style.height = height;
            cell.style.height = height;
        }
    }
});
function TableColumnResizing_onmouseup(e) {
    if(__aspxTableColumnResizing != null) {
        __aspxTableColumnResizing.onmouseup(e);
        return true;
    }
}
function TableColumnResizing_onmousemove(e) {
    if(__aspxTableColumnResizing != null) {
        __aspxTableColumnResizing.onmousemove(e);
        return true;
    }
}
function TableColumnResizing_onselectstart(e) {
    if(__aspxTableColumnResizing != null) {
        document.selection.empty();
        return false;
    }
}

function TableScrollerHelper_AfterWindowResize(index) {
    if(__aspxScrollerHelpers != null && index < __aspxScrollerHelpers.length) {
        __aspxScrollerHelpers[index].AfterWindowResize();
    }
}

function TableScrollerHelper_windowResize(e) {
    var width = document.body.offsetWidth;
    if(width == ASPxClientTableScrollerHelper.prevBodyWidth)
        return;
    ASPxClientTableScrollerHelper.prevBodyWidth = width;    
    if(__aspxScrollerHelpers != null) {
        for(var i = 0; i < __aspxScrollerHelpers.length; i ++) {
            __aspxScrollerHelpers[i].OnWindowResize(i);
        }
    }
	for(var name in ASPxClientGridView.instances) {
		var obj = ASPxClientGridView.instances[name];
		if(obj.UpdateHorzScrollDivs)
			obj.UpdateHorzScrollDivs(true);
	}
}

_aspxAttachEventToDocument("mouseup", TableColumnResizing_onmouseup);
_aspxAttachEventToDocument("mousemove", TableColumnResizing_onmousemove);
_aspxAttachEventToDocument("selectstart", TableColumnResizing_onselectstart);
_aspxAttachEventToElement(window, "resize", TableScrollerHelper_windowResize);