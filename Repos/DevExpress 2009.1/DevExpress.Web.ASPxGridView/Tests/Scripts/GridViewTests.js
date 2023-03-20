/******************************************************
*******************************************************/
TestASPxClientGridView = _aspxCreateClass(ASPxClientGridView, {
	constructor: function(name){
		this.constructor.prototype.constructor.call(this, name);
		this.testSelInput = {value: ''};
		this.testPageRowCount = 10;
		this.pageRowCount = this.testPageRowCount;
    },
	GetSelectionInput: function() { return this.testSelInput; },
	GetDataRow: function(visibleIndex) { return -1; }
});

newTest("SelectionTest").Execute = function() {
    var grid = new TestASPxClientGridView("grid");
	this.AreEqual(false, grid._isRowSelected(0));
	this.AreEqual(0, grid._getSelectedRowCount());
    grid.SelectRow(0, true);
	this.AreEqual(true, grid._isRowSelected(0), "Selected");
	this.AreEqual(1, grid._getSelectedRowCount());
	
    grid.SelectRow(0, false);
	this.AreEqual(false, grid._isRowSelected(0), "Not selected");
	this.AreEqual(0, grid._getSelectedRowCount());
}
newTest("SelectionAllTest").Execute = function() {
    var grid = new TestASPxClientGridView("grid");
	this.AreEqual(0, grid._getSelectedRowCount());
	this.AreEqual(true, grid.pageRowCount > 0);
    grid._selectAllRowsOnPage(true);
	this.AreEqual(grid.pageRowCount, grid._getSelectedRowCount());
    grid._selectAllRowsOnPage(false);
	this.AreEqual(0, grid._getSelectedRowCount());
	
}
newTest("GetDataRowIndex").Execute = function() {
   var grid = new TestASPxClientGridView("grid");
   var res = grid.getRowIndex("DXDataRow12");
	this.AreEqual(15, res + 3, "GetDataRowIndex doesn't work");
}
newTest("GetGroupRowIndex").Execute = function() {
   var grid = new TestASPxClientGridView("grid");
   var res = grid.getRowIndex("DXGroupRow12");
	this.AreEqual(15, res + 3, "GetGroupRowIndex doesn't work");
}
newTest("GetColIndex").Execute = function() {
   var grid = new TestASPxClientGridView("grid");
   var res = grid.getColumnIndex("mySupercol4");
	this.AreEqual(9, res + 5, "GetColIndex doesn't work");
}
    

