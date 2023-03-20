var pivotGrid_FieldValueCMParams = new Array();
var pivotGrid_HeaderCMParams = new Array();
var pivotGrid_AllowedAreaIds = new Array();
var pivotGrid_Groups = new Array();

ASPxRect = _aspxCreateClass(null, {
	constructor: function(left, top, right, bottom) {
		this.left = left;
		this.top = top;
		this.right = right;
		this.bottom = bottom;
	},
	Contains: function(x, y) {
		return this.left < x && x < this.right && this.top < y && y < this.bottom;
	},
	Intersects: function(rect) {
		if(rect.left > this.right || rect.top > this.bottom || 
			this.left > rect.right || this.top > rect.bottom) return false;
		return true;
	},
	ToString: function() {
		return "{" + this.left + "," + this.top + "," + this.right + "," + this.bottom + "}";
	}
});

function _aspxGetBounds(elem) {
	 var left = _aspxGetAbsoluteX(elem),
		top = _aspxGetAbsoluteY(elem),
		right = left + elem.offsetWidth,
		bottom = top + elem.offsetHeight;
	return new ASPxRect(left, top, right, bottom);
}
ASPxClientPivotGridGroup = _aspxCreateClass(null, {
	constructor: function(pivotName, fields) {
		this.pivotName = pivotName;
		this.fields = fields;
		this.count = fields.length;
	},
	ContainsField: function(fieldId) {
	    return this.IndexOf(fieldId) >= 0;
	},
	IsInnerField: function(fieldId) {
	    var index = this.IndexOf(fieldId);
	    return index > 0 && index < this.count - 1 && this.count >= 2;
	},
	IndexOf: function(fieldId) {
	    for(var i = 0; i < this.count; i++) {
	        if(this.GetFieldId(i) == fieldId)
	            return i;
	    }
	    return -1;
	},
	GetFieldId: function(index) {
	    return this.pivotName + "_" + this.fields[index];
	}
});

ASPxClientPivotGrid = _aspxCreateClass(ASPxClientControl, {
	constructor: function(name){
	    this.constructor.prototype.constructor.call(this, name);
	    
        this.filterValues = "";
        this.filterPersister = "";
        this.filterFieldIndex = "";
        this.headerMenuHideColumn = "";
        this.fieldMenuCellState = "";
        this.CustomizationFieldsVisibleChanged = new ASPxClientEvent();
        this.AfterCallback = new ASPxClientEvent();
        this.BeforeCallback = new ASPxClientEvent();
        this.CellClick = new ASPxClientEvent();
        this.CellDblClick = new ASPxClientEvent();
        this.PopupMenuItemClick = new ASPxClientEvent();
        this.Targets = new Object();
        this.isCallbackInProcess = false;
    },
    GetChildElementById: function(childName){
        if(!_aspxIsExists(this.childrenCache)) 
            this.childrenCache = new Object();
        if(!_aspxIsExistsElement(this.childrenCache[childName])) 
            this.childrenCache[childName] = _aspxGetElementById(this.name + "_" + childName);
        return this.childrenCache[childName];
    },
    GetMainElement: function() { return this.GetChildElementById("ME"); }, 
    GetMainDiv: function() { return this.GetChildElementById("MTD"); },        
    GetMainTable: function() { return this.GetChildElementById("MT"); },
    GetEmptyAreaCell: function(intAreaID) { return this.GetChildElementById("pgArea" + intAreaID); },
    GetHeadersTable: function(stringAreaID) { return this.GetChildElementById("ACC" + stringAreaID); },
    GetFilterWindowContent: function(){ return this.GetChildElementById("FPC"); },
    GetPager: function(){  return this.GetChildElementById("PGR"); },
    GetArrowDragDownImage: function(){  return this.GetChildElementById("IADD"); },
    GetArrowDragUpImage: function(){  return this.GetChildElementById("IADU"); },
    GetDragHideFieldImage: function(){  return this.GetChildElementById("IDHF"); },
    GetGroupSeparatorImage: function(){  return this.GetChildElementById("IGS"); },
    GetResizerImage: function(){  return this.GetChildElementById("FPWR"); },
    GetCustomizationFieldsWindow: function() {
        return aspxGetPopupControlCollection().Get(this.name + "_DXCustFields");
    },
    GetCustomizationFieldsWindowElement: function() {
        return this.GetCustomizationFieldsWindow().GetWindowElement(-1);
    },
    GetCustomizationFieldsWindowContentDiv: function() {
        return this.GetChildElementById("DXCustFields_ContentDiv");
    },
    GetPrefilterWindow: function() {
        return aspxGetPopupControlCollection().Get(this.name + "_DXPFCForm");
    },
    GetFilterControl: function() {
		return aspxGetControlCollection().Get(this.name + "_DXPFCForm_DXPFC");
    },
    GetDataHeadersPopupCell: function() { return this.GetChildElementById("DHPC"); },
    GetDataHeadersPopup: function() { 
		return aspxGetPopupControlCollection().Get(this.name + "_DHP");
    },
    GetDataHeadersPopupWindowElement: function() { 
		var popupControl = this.GetDataHeadersPopup();
		if(popupControl == null) return null;
		return popupControl.GetWindowElement(-1);
    },
    IsDataHeadersPopupExists: function() {
		var headersPopup = this.GetDataHeadersPopup();
		if(!_aspxIsExists(headersPopup)) return false;
		var headersPopupWindow = headersPopup.GetWindowElement(-1);
		return _aspxIsExists(headersPopupWindow) && _aspxIsExists(headersPopupWindow.id);
    },
    Initialize: function(){
		this.constructor.prototype.Initialize.call(this);
    
        if(this.__aspxPivotFilterWindow == null) {
            this.__aspxPivotFilterWindow = new ASPxClientPopupFilterWindow(this.name);
            this.__aspxPivotFilterWindow.onHidden = this.filterPopupHidden;
        }
        var resizerImage = this.GetResizerImage();
        if(resizerImage != null) 
            resizerImage.ondragstart = function() { this.releaseCapture(); return false; };
        this.AfterCallBackInitialize();        
    },   
    AfterInitialize: function() {		
		this.constructor.prototype.AfterInitialize.call(this);
		if(this.IsDataHeadersPopupExists()) {
			var headersPopup = this.GetDataHeadersPopup();
			headersPopup.SetSize(1, 1);
			if(headersPopup.IsVisible()) 
				headersPopup.Hide();
        }
    },    
    AfterCallBackInitialize: function() {
        this.AssignContextMenus(pivotGrid_FieldValueCMParams, pivotGrid_FieldValueContextMenuHandler);
        this.AssignContextMenus(pivotGrid_HeaderCMParams, pivotGrid_HeaderContextMenuHandler);
    },
    AssignContextMenus: function(cmParams, handler) {
        if(_aspxIsExists(cmParams) && _aspxIsExists(cmParams[this.name])) {
            var params = cmParams[this.name];
            for(var i = 0; i < params.length; i++) {
                var id = params[i][0];
                var elem = this.GetChildElementById(id);
                if(elem != null) {
                    elem.contextMenuParams = params[i];
                    elem.pivotClientID = this.name;
                    _aspxAttachEventToElement(elem, "contextmenu", handler);   
                }                    
            }            
        }
    },
    CreateLoadingPanel: function(content){
        if(content != null){
			content.innerHTML = '';
            var element = this.CreateLoadingPanelInsideContainer(content);
            if(element != null) {
					element.style.borderWidth = 0;
				for(var i = 0; i < element.childNodes.length; i++) {
					var child = element.childNodes[i];
					if(_aspxIsExists(child.style))
						child.style.border = "0";
				}
            }
        }
    },
    ShowFilterPopup: function(headerId, index) {
        var headerElement = _aspxGetElementById(headerId);
        var fwContent = this.GetFilterWindowContent();
        if(headerElement == null || fwContent == null) return;
        this.__aspxPivotFilterWindow.Show(headerElement, this.GetMainElement());
        if(!_aspxIsExists(fwContent.dxFieldIndex) || fwContent.dxFieldIndex != index) {
            this.CreateLoadingPanel(fwContent);
            this.CreateCallback("FS|" + index);
        }
    },
    filterPopupHidden: function(filterWindow) {
        var fwContent = filterWindow.GetElement();
        if(fwContent == null) return;
        var innerHtml = fwContent.innerHTML;
        if(innerHtml.length > 100000) {
            fwContent.innerHTML = "";
            fwContent.dxFieldIndex = -1;
        }
    },
    SaveContentDivScrollTop: function() {
        var contentDiv = this.GetCustomizationFieldsWindowContentDiv();
        if(contentDiv != null)
            this.contentDivScrollTop = contentDiv.scrollTop;
        else
            this.contentDivScrollTop = -1;
    },
    RestoreContentDivScrollTop: function() {
        if(!_aspxIsExists(this.contentDivScrollTop) || this.contentDivScrollTop < 0) return;
        var contentDiv = this.GetCustomizationFieldsWindowContentDiv();        
        if(contentDiv != null) {
            contentDiv.scrollTop = this.contentDivScrollTop;
            this.contentDivScrollTop = -1;
        }
    },
    PerformCallbackInternal: function(el, arg) {
        this.isCallbackInProcess = true;
        if(this.callBacksEnabled) {
            this.OnBeforeCallback(el);
            this.CreateCallback(arg);
        }
        else
            this.SendPostBack(arg);
    },
    OnCallback: function(result) {
        this.isCallbackInProcess = false;
		this.ChangePrefilterVisibilityClientSize(false);
        var value = new String(result);
        var separatorIndex = value.indexOf("|");
        var id = value.substr(0, separatorIndex),
            argument = value.substr(separatorIndex + 1);
        if(id == "F") 
            this.doFilterCallback(argument);
        if(id == "G") 
            this.doGridRefreshCallback(argument);
        this.OnAfterCallback();
    },
    OnCallbackError: function(result, data) {
        this.isCallbackInProcess = false;
        this.constructor.prototype.OnCallbackError.call(this, result, data);
		this.OnAfterCallback();
    },
    OnBeforeCallback: function(el) {
        var mainDiv = this.GetMainDiv();
        if(mainDiv == null) return; 
        
        this.SaveContentDivScrollTop();
        
        var mainElement = this.GetMainElement();
        this.CreateLoadingPanelWithAbsolutePosition(mainDiv, mainElement);
        this.CreateLoadingDiv(mainDiv, mainElement);
                
        if(_aspxIsExists(this.RaiseBeforeCallback))
		    this.RaiseBeforeCallback();
    },
    OnAfterCallback: function() {
        this.childrenCache = new Object();    
		this.isInitialized = false;
		
		setTimeout("pivotGrid_RaiseAfterCallback('" + this.name + "');", 0);
    },
    GetCallbackState: function() {
		var elem = this.GetChildElementById("CallbackState");
		if(elem == null) return "";
		return "CB|" + elem.value;
    },
    doFilterCallback: function(argument) {
        var values = argument.split("|");
        if(values.length != 4) return;
        var fwContent = this.GetFilterWindowContent();
        if(fwContent == null) return;
        this.filterValues = values[0];
        this.filterPersister = values[1];
        this.filterFieldIndex = values[2];
        fwContent.dxFieldIndex = this.filterFieldIndex;
        _aspxSetInnerHtml(fwContent, values[3]);
    },
    doGridRefreshCallback: function(argument) {        
        var mainDiv = this.GetMainDiv();
        if(mainDiv == null) return;
        _aspxSetInnerHtml(mainDiv, argument);
    },
    isAllFilterValuesChecked: function() {
		var i = 0;
		while(_aspxIsExists(checkBox = document.getElementById(this.name + 'FTRI' + i))) {
			if(!checkBox.checked) return false;
			i++;
		}
		return true;
    },
    filterValueChanged: function(index) {
		var showAllCheckBox = document.getElementById(this.name + 'FTRIAll');
		if(index < 0) {
			var i = 0, checkBox;
			var newFilterValues = '', newChar = showAllCheckBox.checked ? 'T' : 'F';			
			while(_aspxIsExists(checkBox = document.getElementById(this.name + 'FTRI' + i))) {
				checkBox.checked = showAllCheckBox.checked;
				newFilterValues += newChar;
				i++;
			}	
			this.filterValues = newFilterValues;		
		} else {
			var newChar = this.filterValues.charAt(index) == 'T' ? 'F' : 'T';
			this.filterValues = this.filterValues.substr(0, index) + newChar + this.filterValues.substr(index + 1, this.filterValues.length - index - 1);			
			showAllCheckBox.checked = this.isAllFilterValuesChecked();
        }
        var btn = this.GetChildElementById("FPWOK");
		if(btn != null)  {
			btn.disabled = this.filterValues.indexOf("T") == -1;
		}
    },
    applyFilter: function() {
        this.PerformCallbackInternal(this.GetMainTable(), "F|" + this.filterValues + '|' + this.filterPersister + '|' + this.filterFieldIndex);
        if(this.__aspxPivotFilterWindow != null)
		    this.__aspxPivotFilterWindow.Hide();
    },
    headerMouseDown: function(root, e) {
        if(!_aspxGetIsLeftButtonPressed(e)) return;
	    var drag = new ASPxClientDragHelper(e, root, true);
	    var targets = new ASPxClientCursorTargets();
	    this.configureDrag(drag, targets);
	    this.configureTargets(targets, drag);
	    this.registerTargets(targets, drag.obj);
    },
    configureDrag: function(drag, targets) {        
		drag.pGrid = this;
		drag.targets = targets;
		drag.onCloneCreating = this.cloneCreating;
        drag.onCloneCreated = this.cloneCreated;
        drag.onEndDrag = this.endDrag;
    },
    configureTargets: function(targets, drag) {
	    targets.pGrid = this;
	    targets.drag = drag;
	    targets.onTargetChanging = this.targetChanging;
	    targets.onTargetChanged = this.targetChanged;
    },
    registerTargets: function(targets, dragObj) {
	    targets.addElement(this.GetCustomizationFieldsWindowElement());
	    targets.addElement(this.GetEmptyAreaCell(0));
	    targets.addElement(this.GetEmptyAreaCell(1));
	    targets.addElement(this.GetEmptyAreaCell(2));
	    targets.addElement(this.GetEmptyAreaCell(3));	   	    
	    
	    var targetIds = new Array();
	    targetIds.push("pgGroupHeader", "pgHeader");
	    targets.onTargetAdding = this.targetAdding;
	    targets.RegisterTargets(this.GetDataHeadersPopupWindowElement(), targetIds);
	    targets.RegisterTargets(this.GetHeadersTable("RowArea"), targetIds);	    
        targets.RegisterTargets(this.GetHeadersTable("ColumnArea"), targetIds);
        targets.RegisterTargets(this.GetHeadersTable("DataArea"), targetIds);
        targets.RegisterTargets(this.GetHeadersTable("FilterArea"), targetIds);
	    targets.UnregisterTargets(dragObj, targetIds);
	    targets.onTargetAdding = null;
	    
	    targets.dataHeadersPopup = this.IsDataHeadersPopupExists() ? this.GetDataHeadersPopup() : null;
	    
	    if(targets.dataHeadersPopup != null) {
			targets.dataHeadersPopupCell = this.GetDataHeadersPopupCell();
			targets.dataHeadersPopupCellRect = _aspxGetBounds(targets.dataHeadersPopupCell);
		}
    },
    targetAdding: function(sender, element) {
        var id = sender.pGrid.getLastIdPart(element.id);
        if(id.length == 0) return false;
        var lastChar = id.charAt(id.length - 1);
        return lastChar >= '0' && lastChar <= '9' && 
               (id.indexOf("pgGroupHeader") >= 0 
                    || (id.indexOf("pgHeader") >= 0 && !sender.pGrid.isInnerRowAreaGroupField(element.id))
               );
    },
    isInnerRowAreaGroupField: function(id) {
        var groups = this.getGroups();
		if(groups != null) {
		    for(var i = 0; i < groups.length; i++) {
        	    if(!groups[i].ContainsField(id)) 
        	        continue;
        	    return groups[i].IsInnerField(id);
            }
        }
        return false;
    },
    isInnerGroupTarget: function(targetElement, isLeft) {
        if(targetElement == null) return false;
        var id = targetElement.id;
        var groups = this.getGroups();
		if(groups == null) return false;	
	    for(var i = 0; i < groups.length; i++) {
    	    var index = groups[i].IndexOf(id);
    	    if(index < 0) continue;
    	    return groups[i].count >= 2 && 
    	        ((index > 0 && index < groups[i].count - 1) || 
    	         (index == 0 && !isLeft) || 
    	         (index == groups[i].count - 1 && isLeft));
        }
        return false;
    },
    getLastIdPart: function(id) {
        var separatorIndex = id.lastIndexOf('_');
        return separatorIndex >= 0 ? id.substr(separatorIndex + 1) : id;
    },
    getGroups: function() {
        return pivotGrid_Groups[this.name];
    },
    headerClick: function(element) {
        if(element.id.indexOf("sorted") > 0){
			this.ResetFilterCache(this.filterFieldIndex);
            this.PerformCallbackInternal(element, 'S|' + element.id);
        }
    },
    ResetFilterCache: function(indexToReset) {
        var fwContent = this.GetFilterWindowContent();
        if(fwContent != null && (!_aspxIsExists(indexToReset) || indexToReset == fwContent.dxFieldIndex))
		    fwContent.dxFieldIndex = -1;
    },
    cloneCreating: function(clone) {
        var pGrid = __aspxDragHelper.pGrid;
        var groups = pGrid.getGroups();
		if(groups == null) return clone;
		for(var i = 0; i < groups.length; i++) {
        	if(groups[i].ContainsField(clone.id)) {
        	    pGrid.correctDragObj(groups[i]);
        	    return pGrid.cloneGroup(groups[i]);
        	}
        }
        return clone;
    },
    correctDragObj: function(group) {
        __aspxDragHelper.obj = this.GetChildElementById(group.fields[0]);
        __aspxDragHelper.dragDiv.style.left = _aspxGetAbsoluteX(__aspxDragHelper.obj) + "px";
        __aspxDragHelper.dragDiv.style.top = _aspxGetAbsoluteY(__aspxDragHelper.obj) + "px";
    },
    cloneGroup: function(group) {
        var separatorWidth = this.getSeparatorWidth(group);    
        var table = document.createElement("table");
        table.cellPadding = 0;
        table.cellSpacing = 0;
        var row = table.insertRow(0);
        for(var i = 0; i < group.count; i++) {
            var header = this.GetChildElementById(group.fields[i]);
            if(header == null) continue;
            var cell = row.insertCell(row.cells.length);
            cell.appendChild(this.cloneHeader(header));
            if(i != group.count - 1) {
                cell = row.insertCell(row.cells.length);                
                cell.appendChild(this.cloneGroupSeparator(separatorWidth));
            }
        }
        return table;
    },
    getSeparatorWidth: function(group) {
        if(group.count < 2) return 0;
        var header0 = this.GetChildElementById(group.fields[0]),
            header1 = this.GetChildElementById(group.fields[1]);
        if(header0 == null || header1 == null) return 0;
        return _aspxGetAbsoluteX(header1) - _aspxGetAbsoluteX(header0) - header0.offsetWidth;
    },
    cloneHeader: function(header) {
        var clone = header.cloneNode(true);
        clone.style.width = header.offsetWidth + "px";
        return clone;
    },
    cloneGroupSeparator: function(separatorWidth) {
        // this.GetGroupSeparatorImage().cloneNode() doesn't work in IE7- produces empty image
        var groupSeparator = this.GetGroupSeparatorImage();
        var clone = document.createElement("img");
        clone.src = groupSeparator.src;
        clone.width = separatorWidth;
        clone.height = groupSeparator.height;
        return clone;
    },
    cloneCreated: function(clone) {
        var list = new Array();
        var width = this.obj.offsetWidth + "px";
        clone.style.width = width;
        list.push(clone);
        for(var i = 0; i < clone.childNodes.length; i ++) {
			if(_aspxIsExists(clone.childNodes[i].style))
				clone.childNodes[i].style.width = width;
            list.push(clone.childNodes[i]);            
        }
        for(var i = 0; i < list.length; i ++) {
            if(!_aspxIsExists(list[i].style)) continue;
            var width = new String(list[i].style.width);
            if(width.indexOf("%") > -1) {
                list[i].style.width = "";
            }
        }
    },
    targetChanging: function(targets) {
        targets.targetTag = targets.isLeftPartOfElement();
        if(targets.pGrid.isInnerGroupTarget(targets.targetElement, targets.targetTag)) {            
            targets.targetElement = null;
            return;
        }
        if(targets.pGrid.IsTargetElementArea(targets)) {
            targets.targetTag = true;
        }
        
        if(targets.dataHeadersPopup == null) return;
		var popupControl = targets.dataHeadersPopup;
		if(targets.dataHeadersPopupCellRect.Contains(targets.x, targets.y)) {				
			if(!popupControl.IsVisible()) {				
				popupControl.Show();	
				var windowElem = popupControl.GetWindowElement(-1);			
				popupControl.bounds = _aspxGetBounds(windowElem);
				
				var list = new Array();
				_aspxGetElementsByPartialId(windowElem, 'pgHeader', list);
				for(var i = 0; i < list.length; i++)
					for(var j = 0; j < targets.list.length; j++) {
						if(targets.list[j].targetElement == list[i]) {
							targets.list[j].absoluteX = _aspxGetAbsoluteX(list[i]);
							targets.list[j].absoluteY = _aspxGetAbsoluteY(list[i]);
							break;
						}
					}
			}
		} else {
			if(_aspxIsExists(popupControl.bounds) && popupControl.bounds != null && popupControl.IsVisible()) {
				if(popupControl.bounds.Contains(targets.x, targets.y)) {
					if(_aspxIsExists(targets.targetElement) && targets.targetElement.id.indexOf("DHP") < 0) 
						targets.cancelChanging();
				} else {				
					popupControl.Hide();
					popupControl.bounds = null;									
				}
			}
		}
    },
    IsTargetElementArea: function(targets) {
        return targets.targetElement != null && targets.targetElement.id.indexOf("_pgArea") > -1;
    },
    IsTargetAllowed: function(targets) {
		var allAllowedIds = pivotGrid_AllowedAreaIds[targets.pGrid.name];
		if(allAllowedIds == null) return true;
        var allowedIds = allAllowedIds[targets.drag.obj.id];
        if(allowedIds == null) return true;
        return _aspxArrayIndexOf(allowedIds, targets.targetElement.id) > -1;
    },
    targetChanged: function(targets) {
       if(__aspxDragHelper == null) return;
       if(targets.targetElement == __aspxDragHelper.obj) return;
       if(targets.targetElement != null && targets.pGrid.IsTargetAllowed(targets)) {
            targets.pGrid.setDragImagesPosition(targets.targetElement, targets.targetTag);
       } else {
            targets.pGrid.targetImagesChangeVisibility("hidden");
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
    setDragImagesPosition: function(el, isLeft) {
        this.targetImagesChangeVisibility("hidden");
        if(el == this.GetCustomizationFieldsWindowElement()) {
            __aspxDragHelper.addElementToDragDiv(this.GetDragHideFieldImage());
        } else {
			var mainElement = this.GetMainElement();
			var arrowUp = this.GetArrowDragUpImage();
            var arrowDown = this.GetArrowDragDownImage();
            var left = 0, top = 0;
			if(el.id.indexOf("DHP") < 0) {
				left = _aspxGetAbsoluteX(el);// - _aspxGetIEDocumentClientOffset(true);
				top = _aspxGetAbsoluteY(el);// - _aspxGetIEDocumentClientOffset(false);
			} else {
				var windowElem = this.GetDataHeadersPopupWindowElement();
				left = _aspxGetAbsoluteX(el) - _aspxGetIEDocumentClientOffset(true);
				top = _aspxGetAbsoluteY(el) - _aspxGetIEDocumentClientOffset(false);
				arrowDown.style.zIndex = windowElem.style.zIndex + 1;
				arrowUp.style.zIndex = windowElem.style.zIndex + 1;
			}
			if(!isLeft)
				left += el.offsetWidth;
            this.targetImagesChangeVisibility("visible");

			var x = left - (arrowDown.offsetWidth / 2);
			x = isLeft ? Math.floor(x) : Math.ceil(x);
            _aspxSetAbsoluteX(arrowDown, x);
            _aspxSetAbsoluteY(arrowDown, (top - arrowDown.offsetHeight));
            _aspxSetAbsoluteX(arrowUp, x);
            _aspxSetAbsoluteY(arrowUp, (top + el.offsetHeight));
        }
    },
    endDrag: function(drag) {
		drag.pGrid.targetImagesChangeVisibility("hidden");
		
        if(drag.targets == null || drag.targets.targetElement == null) return;
        drag.pGrid.PerformCallbackInternal(drag.targets.targetElement, 'D' + '|' + drag.obj.id + '|' + drag.targets.targetElement.id + '|' + drag.targets.targetTag);
    },
    HideCustomizationFields: function() {
        this.ChangeCustomizationFieldsVisibilityInternal(false, true);
    },
    ShowCustomizationFields: function(animate) {
        this.ChangeCustomizationFieldsVisibilityInternal(true, animate);
    },
    IsCustomizationFieldsVisible: function() {
        var fieldsWindow = this.GetCustomizationFieldsWindow();
        if(fieldsWindow == null) return false;
        return fieldsWindow.IsVisible();
    },
    ChangeCustomizationFieldsVisibilityInternal: function(visible, animate) {
        if(!_aspxIsExists(visible)) {
            visible = !this.IsCustomizationFieldsVisible();
        }
        var fieldsWindow = this.GetCustomizationFieldsWindow();
        if(fieldsWindow == null) return;
        var oldEnableAnimation = fieldsWindow.enableAnimation;
        fieldsWindow.enableAnimation = animate;
        if(visible) {
            fieldsWindow.Show();
        } else {
            fieldsWindow.Hide();
        }
        fieldsWindow.enableAnimation = oldEnableAnimation;
    },
    IsPrefilterVisible: function() {
        var prefilterPopup = this.GetPrefilterWindow();
        return prefilterPopup != null && prefilterPopup.GetVisible();
    },
    ShowPrefilter: function() {
        this.ChangePrefilterVisibility(true);
    },
    ApplyPrefilter: function() {
		var fc = this.GetFilterControl();				
		if(fc == null) return;
		fc.Apply(this);
    },
    HidePrefilter: function() {
        this.ChangePrefilterVisibility(false);
    },
    ChangePrefilterVisibility: function(visible) {
		this.ChangePrefilterVisibilityClientSize(visible);
        this.PerformCallbackInternal(this.GetMainTable(), "PREFILTER|" + (visible ? "Show" : "Hide"));
    },
    ChangePrefilterVisibilityClientSize: function(visible) {
        if(!visible) {
            var prefilterPopup = this.GetPrefilterWindow();
            if(prefilterPopup != null)
                prefilterPopup.Hide();
        }
    },
    ClearPrefilter: function() {
        this.PerformCallbackInternal(this.GetMainTable(), "PREFILTER|Reset");
    },
    ChangePrefilterEnabled: function() {
        this.PerformCallbackInternal(this.GetMainTable(), "PREFILTER|ChangeEnabled");
    },
    showHeaderMenu: function(e, hideColumn, fieldID) {
        this.headerMenuHideColumn = hideColumn;
        this.headerMenuFieldID = fieldID;
        var menu = aspxGetControlCollection().Get(this.name + "_HM");
        if(menu == null) return;
        this.SetMenuItemVisibilityState(menu, "Hide", hideColumn != "");
        this.SetMenuItemVisibilityState(menu, "HideList", this.IsCustomizationFieldsVisible());
		this.SetMenuItemVisibilityState(menu, "ShowList", !this.IsCustomizationFieldsVisible());
		this.SetMenuItemVisibilityState(menu, "ShowPrefilter", !this.IsPrefilterVisible());
			
        this.showMenu(e, menu);
    },
    SetMenuItemVisibilityState: function(menu, name, isVisible) {
		menuItem = menu.GetItemByName(name);
        if(menuItem != null )
			menuItem.SetVisible(isVisible);
    },
    onHeaderMenuClick: function(itemName) {
        switch(itemName) {
            case "Refresh": this.PerformCallbackInternal(this.GetMainTable(), "RELOAD"); break;
            case "Hide": this.PerformCallbackInternal(this.GetMainTable(), this.headerMenuHideColumn); break;
            case "HideList": this.HideCustomizationFields(); break;
            case "ShowList": this.ShowCustomizationFields(true); break;
            case "ShowPrefilter": this.ShowPrefilter(); break;
            default: 
				if(_aspxIsExists(this.RaisePopupMenuItemClick))
					this.RaisePopupMenuItemClick("HeaderMenu", itemName, this.headerMenuFieldID, -1);
				break;
        }
    },
    showFieldValueMenu: function(e, state, iscollapsed, fieldID, itemIndex, isLastLevel, area, sortedFields, dataIndex) {
        this.fieldMenuCellState = state;
        this.fieldMenuFieldID = fieldID;
        this.fieldMenuFieldValueIndex = itemIndex;
        this.fieldMenuDataIndex = dataIndex;
        var menu = aspxGetControlCollection().Get(this.name + "_FVM");
        if(menu == null) return;            
        this.FilterFieldValueMenuItems(menu, state, iscollapsed, isLastLevel, area, sortedFields, dataIndex);
        if(this.MenuHasVisibleItems(menu))
			this.showMenu(e, menu);
    },
    MenuHasVisibleItems: function(menu) {
		var itemsCount = menu.GetItemCount();        
        for(var i = 0; i < itemsCount; i++) {
            var menuItem = menu.GetItem(i);
            if(menuItem.GetVisible())
				return true;
        }
        return false;
    },
    FilterFieldValueMenuItems: function(menu, state, iscollapsed, isLastLevel, area, sortedFields, dataIndex) {
        this.SetMenuItemVisibilityState(menu, "Expand", iscollapsed && state != "");
        this.SetMenuItemVisibilityState(menu, "Collapse", !iscollapsed && state != "");
        this.SetMenuItemVisibilityState(menu, "ExpandAll", state != "");
        this.SetMenuItemVisibilityState(menu, "CollapseAll", state != "");
        
        var itemsCount = menu.GetItemCount(),
			showRemoveAll = false;           
        for(var i = 0; i < itemsCount; i++) {
            var menuItem = menu.GetItem(i);
            if(menuItem.name.indexOf("SortBy_") == 0) {
				var isRemoveAll = menuItem.name.indexOf("RemoveAll") >= 0;
                var visible = isLastLevel && menuItem.name.indexOf(area) > 0;
                if(isRemoveAll) {
					visible = visible && showRemoveAll;
                } else {
					if(dataIndex >= 0)
						visible = visible && menuItem.name.indexOf("_" + dataIndex) == menuItem.name.lastIndexOf("_");
				}
                menuItem.SetVisible(visible); 
                if(visible && !isRemoveAll) {
					var isChecked = this.GetSortByMenuItemCheckedState(menuItem, sortedFields);
					showRemoveAll |= isChecked;
                    menuItem.SetChecked(isChecked);
                }
            }
        }
    },
    GetSortByMenuItemCheckedState: function(menuItem, sortedFields) {
        if(menuItem.name.indexOf("RemoveAll") >= 0) return false;
        for(var i = 0; i < sortedFields.length; i++) {
            if(menuItem.name.indexOf(sortedFields[i]) >= 0) 
                return true;
        }
        return false;
    },   
    showMenu: function(e, menu) {
        menu.ShowInternal(e);
        e.returnValue = false;
        e.cancelBubble = true;
    },
    onFieldValueMenuClick: function(itemName) {
        switch(itemName) {
            case "Expand": 
            case "Collapse": this.PerformCallbackInternal(this.GetMainTable(), this.fieldMenuCellState); break;
            case "ExpandAll": this.PerformCallbackInternal(this.GetMainTable(), this.fieldMenuCellState + "|EA"); break;
            case "CollapseAll": this.PerformCallbackInternal(this.GetMainTable(), this.fieldMenuCellState + "|CA"); break;
            default: 
                if(itemName.indexOf("SortBy_") == 0)
                    this.onSortByFieldValueMenuClick(itemName);
                else {
				    if(_aspxIsExists(this.RaisePopupMenuItemClick))
					    this.RaisePopupMenuItemClick("FieldValueMenu", itemName, this.fieldMenuFieldID, this.fieldMenuFieldValueIndex);
				}
				break;
        }
    },
    onSortByFieldValueMenuClick: function(itemName) {
        var argument = this.GetSortByArgument(itemName);
        this.PerformCallbackInternal(this.GetMainTable(), "SS|" + this.fieldMenuFieldID + "|" + this.fieldMenuFieldValueIndex + "|" + argument);
    },
    GetSortByArgument: function(itemName) {
        var sortByStr = "SortBy_";
        var startIndex = itemName.indexOf("_", sortByStr.length + 1) + 1;
        var res = itemName.substr(startIndex).replace("_", "|");
        if(res == "RemoveAll")
            res += "|" + this.fieldMenuDataIndex;
        return res;
    },
    DoPagerClick: function(element, value) {
        this.PerformCallbackInternal(element, "P|" + value);
    },    
    RaiseCustomizationFieldsVisibleChanged: function() {
	    if(!this.CustomizationFieldsVisibleChanged.IsEmpty()){
		    var args = new ASPxClientEventArgs();            
		    this.CustomizationFieldsVisibleChanged.FireEvent(this, args);
	    }
    },    
    RaiseCellClick: function(value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex) {
	    if(!this.CellClick.IsEmpty()){
		    var args = new ASPxClientClickEventArgs(value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex);
		    this.CellClick.FireEvent(this, args);
	    }
    },    
    RaiseCellDblClick: function(value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex) {
	    if(!this.CellDblClick.IsEmpty()){
		    var args = new ASPxClientClickEventArgs(value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex);
		    this.CellDblClick.FireEvent(this, args);
	    }
    },    
    RaiseAfterCallback: function() {
	    if(!this.AfterCallback.IsEmpty()){
		    var args = new ASPxClientEventArgs();            
		    this.AfterCallback.FireEvent(this, args);
	    }
    },    
    RaiseBeforeCallback: function() {
	    if(!this.BeforeCallback.IsEmpty()){
		    var args = new ASPxClientEventArgs();            
		    this.BeforeCallback.FireEvent(this, args);
	    }
    },  
    RaisePopupMenuItemClick: function(menuType, menuItemName, fieldID, fieldValueIndex) {
	    if(!this.PopupMenuItemClick.IsEmpty()){
		    var args = new ASPxClientPivotMenuItemClickEventArgs(menuType, menuItemName, fieldID, fieldValueIndex);            
		    this.PopupMenuItemClick.FireEvent(this, args);
	    }
    },
    GetCustomizationFieldsVisibility: function() {
        return this.IsCustomizationFieldsVisible();
    },
    SetCustomizationFieldsVisibility: function(value) {
        this.ChangeCustomizationFieldsVisibilityInternal(value);
    },
    ChangeCustomizationFieldsVisibility: function() {
        this.ChangeCustomizationFieldsVisibilityInternal();
    },
    PerformCallback: function(args){
        if(!_aspxIsExists(args)) args = "";
        this.ResetFilterCache();
        this.PerformCallbackInternal(this.GetMainTable(), "C|" + args);
    }
});

function pivotGrid_GetGrid(id) {
    return aspxGetControlCollection().Get(id);
}
function pivotGrid_IsBusy(id) {
    var grid = pivotGrid_GetGrid(id);
    if(grid.isCallbackInProcess)
        return true;
    var filterControl = grid.GetFilterControl();
    if(filterControl != null && filterControl.isCallbackInProcess)
        return true;
    return false;
}
function pivotGrid_GetGridFromMenu(menu) {
    if(menu == null) return null;
    var pos = menu.name.lastIndexOf("_");
    if(pos > -1) {
        return pivotGrid_GetGrid(menu.name.substring(0, pos));
    } 
    return null;
}
function pivotGrid_OnHeaderMenuClick(s, args) {
    var grid = pivotGrid_GetGridFromMenu(s);
    if(grid != null) {
        grid.onHeaderMenuClick(args.item.name);
    }
}
function pivotGrid_ShowHeaderMenu(id, e, hideColumn, fieldID) {
    var grid = pivotGrid_GetGrid(id);
    if(grid != null) {
        grid.showHeaderMenu(e, hideColumn, fieldID);
    }
}
function pivotGrid_OnFieldValueMenuClick(s, args) {
    var grid = pivotGrid_GetGridFromMenu(s);
    if(grid != null) {
        grid.onFieldValueMenuClick(args.item.name);
    }
}
function pivotGrid_ShowFieldValueMenu(id, e, state, iscollapsed, fieldID, itemIndex, isLastLevel, area, sortedFields, dataIndex) {
    var grid = pivotGrid_GetGrid(id);
    if(grid != null) {   
        grid.showFieldValueMenu(e, state, iscollapsed, fieldID, itemIndex, isLastLevel, area, sortedFields, dataIndex);
    }
}
function pivotGrid_HeaderMouseDown(id, element, e) {
    var grid = pivotGrid_GetGrid(id);
    if(grid != null) {
       grid.headerMouseDown(element, e);            
    }
}
function pivotGrid_HeaderClick(id, element, e) {
    var grid = pivotGrid_GetGrid(id);
    if(grid != null) {
        grid.headerClick(element);
    }
}

function pivotGrid_ShowFilterPopup(name, headerId, index) {
	if(__aspxClientPopupFilterWindowShowAgain) {
		__aspxClientPopupFilterWindowShowAgain = false;
		return;
	}
    var pg = aspxGetControlCollection().Get(name);
    if(pg != null) 
        pg.ShowFilterPopup(headerId, index);
}    

function pivotGrid_PerformCallback(name, el, value) {
    var pg = pivotGrid_GetGrid(name);
    if(pg != null) 
        pg.PerformCallbackInternal(el, value);
}

function pivotGrid_FilterValueChanged(name, index) {
    var pg = pivotGrid_GetGrid(name);
    if(pg != null) 
        pg.filterValueChanged(index);
    else
		alert("PivotGrid was not found");
}

function pivotGrid_ApplyFilter(name) {
    var pg = pivotGrid_GetGrid(name);
    if(pg != null) 
        pg.applyFilter();
    else
		alert("PivotGrid was not found");
}

function pivotGrid_HideFilter(name) {
	var pg = pivotGrid_GetGrid(name);
    if(pg != null) 
        pg.__aspxPivotFilterWindow.Hide();
}

function pivotGrid_ClearSelection(){
    if(!__aspxOpera && !__aspxOpera9) return;
    if (_aspxIsExists(window.getSelection)){
        if (__aspxSafariFamily)
            window.getSelection().collapse();
        else
            window.getSelection().removeAllRanges();
    }
    else if (_aspxIsExists(document.selection)){
        if(_aspxIsExists(document.selection.empty))
            document.selection.empty();
		else if(_aspxIsExists(document.selection.clear))
			document.selection.clear();
    }
}

function pivotGrid_PagerClick(name, element, id) {
    var pg = pivotGrid_GetGrid(name);
    if(pg != null) pg.DoPagerClick(element, id);
}

function pivotGrid_GetGridByCustomizationFields(custFields) {
    if(!_aspxIsExists(custFields.pivotGrid) || custFields.pivotGrid == null) {
        var name = custFields.name.substr(0, custFields.name.length - ("_DXCustFields").length);
	    custFields.pivotGrid = aspxGetControlCollection().Get(name);
	}
	return custFields.pivotGrid;
}

function DoCustomizationFieldsVisibleChanged(sender){
	var control = pivotGrid_GetGridByCustomizationFields(sender);
	if(control == null) return;
	if(_aspxIsExists(control.RaiseCustomizationFieldsVisibleChanged)) {
		control.RaiseCustomizationFieldsVisibleChanged();
	}
}
function DoUpdateContentSize(sender) {
    var contentCell = sender.GetWindowContentElement(-1);
    var content = _aspxGetElementById(sender.name + "_ContentDiv");
    var oldWidth = contentCell.clientWidth,
        oldHeight = contentCell.clientHeight;
    content.style.width = contentCell.clientWidth + "px";
    content.style.height = contentCell.clientHeight + "px";
    var widthDiff = contentCell.clientWidth - oldWidth;
    if(widthDiff != 0)
        content.style.width = (oldWidth - widthDiff) + "px";
    var heightDiff = contentCell.clientHeight - oldHeight;
    if(heightDiff != 0)
        content.style.height = (oldHeight - heightDiff) + "px";
    
    var control = pivotGrid_GetGridByCustomizationFields(sender);
	if(control != null)
	    control.RestoreContentDivScrollTop();
}
function DoResetContentSize(sender) {
    var content = _aspxGetElementById(sender.name + "_ContentDiv");
    content.style.width = "0px";
    content.style.height = "0px"
}

function pivotGrid_FieldValueContextMenuHandler(e) {
    var source = _aspxGetEventSource(e);
    while(source != null && !_aspxIsExists(source.contextMenuParams))
        source = source.parentNode;
    if(source == null)
        return;
    pivotGrid_ShowFieldValueMenu(source.pivotClientID, e, source.contextMenuParams[1], source.contextMenuParams[2],
        source.contextMenuParams[3], source.contextMenuParams[4], source.contextMenuParams[5], source.contextMenuParams[6],
        source.contextMenuParams[7], source.contextMenuParams[8]); 
} 

function pivotGrid_HeaderContextMenuHandler(e) {
    var source = _aspxGetEventSource(e);
    while(source != null && !_aspxIsExists(source.contextMenuParams))
        source = source.parentNode;
    if(source == null)
        return;
    pivotGrid_ShowHeaderMenu(source.pivotClientID, e, source.contextMenuParams[1], source.contextMenuParams[2]); 
}

function pivotGrid_AfterCallBackInitialize(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null) 
        pg.AfterCallBackInitialize();
}

function pivotGrid_RaiseAfterCallback(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null && _aspxIsExists(pg.RaiseAfterCallback)) {
	    pg.RaiseAfterCallback();
    }
}

function pivotGrid_CellClick(id, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null && _aspxIsExists(pg.RaiseCellClick)) 
        pg.RaiseCellClick(value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex);
}

function pivotGrid_CellDoubleClick(id, value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null && _aspxIsExists(pg.RaiseCellDblClick)) 
        pg.RaiseCellDblClick(value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex);
}

function pivotGrid_Sort508(id, headerSuffix) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null) {
		pg.ResetFilterCache(pg.filterFieldIndex);
		pg.PerformCallbackInternal(null, 'S|' + id + "_" + headerSuffix);
	}
}

function pivotGrid_ApplyPrefilter(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null) 
		pg.ApplyPrefilter();
}

function pivotGrid_HidePrefilter(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null) 
		pg.HidePrefilter();
}

function pivotGrid_ShowPrefilter(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null) 
		pg.ShowPrefilter();
}

function pivotGrid_ClearPrefilter(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null) 
		pg.ClearPrefilter();
}

function pivotGrid_ChangePrefilterEnabled(id) {
    var pg = pivotGrid_GetGrid(id);
    if(pg != null) 
		pg.ChangePrefilterEnabled();
}
ASPxClientClickEventArgs = _aspxCreateClass(ASPxClientEventArgs, {
    constructor: function(value, columnIndex, rowIndex, columnValue, rowValue, columnFieldName, rowFieldName, columnValueType, rowValueType, dataIndex){
        this.constructor.prototype.constructor.call();
        this.Value = value;
        this.ColumnIndex = columnIndex;
        this.RowIndex = rowIndex;
        this.ColumnValue = columnValue;
        this.RowValue = rowValue;
        this.ColumnFieldName = columnFieldName;
        this.RowFieldName = rowFieldName;
        this.ColumnValueType = columnValueType;
        this.RowValueType = rowValueType;
        this.DataIndex = dataIndex;
    }
});
ASPxClientPivotMenuItemClickEventArgs = _aspxCreateClass(ASPxClientEventArgs, {
    constructor: function(menuType, menuItemName, fieldID, fieldValueIndex) {
        this.constructor.prototype.constructor.call();
        this.MenuType = menuType;
        this.MenuItemName = menuItemName;
        this.FieldID = fieldID;
        this.FieldValueIndex = fieldValueIndex;
    }
});