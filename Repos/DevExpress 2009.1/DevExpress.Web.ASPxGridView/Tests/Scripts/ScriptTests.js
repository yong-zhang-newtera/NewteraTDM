function /*$test*/ EditorsToBeResizedListCompositionTest() {
    // Utils
    var stub = function() { };
    var testEditor = function(index) { this.index = index; };
    
    // Grid being tested
    var grid = new ASPxClientGridView();

    // Grid methods substitution
    grid._getColumnObjectByArg = stub;
    grid.getColumnIndex = stub;
    grid.RaiseColumnResizing = stub;
    grid.SetResizableControl = stub;
    grid.GetAutoFilterEditor = function() { return new testEditor(1); }
    grid._getEditors = function() { return [ new testEditor(2), new testEditor(3), new testEditor(4) ]; };
    
    // Column resizing methods substitution
    grid.columnResizing = new ASPxClientTableColumnResizing();
    grid.columnResizing.CanStartResizing = function() { return true; };
    grid.columnResizing.StartResizing = stub;
    grid.columnResizing.SetResizedInput = stub;
    grid.columnResizing.SetResizableControl = stub;

    // Testing
    grid.startColumnResizing({}, {});
    for(var i = 0; i < 4; i++) {
        var editor = grid.columnResizing.editorsToBeResized[i];
        Assert.AreEqual(i + 1, editor.index, "Unexpected item in the editors to be resized list.");
    }
}

function /*$test*/ test_validate() {
    this.EditorMock = _aspxCreateClass(null, {
        constructor: function(futureValidationResult, setFocusOnError) {
            this.futureValidationResult = futureValidationResult;
            this.setFocusOnError = setFocusOnError;
            this.isValid = true;
            this.focused = false;
        },
        Validate: function() { this.isValid = this.futureValidationResult; },
        GetIsValid: function() { return this.isValid; },
        Focus: function() { this.focused = true; }
    });
    var list = [
        new this.EditorMock(true),
        new this.EditorMock(false, false), 
        new this.EditorMock(false, true), // focus expected
        new this.EditorMock(false, true)
    ];
    var validationResult = grid._validate(list);
    
    Assert.IsFalse(validationResult, "Invalid validation result");
    Assert.IsTrue(list[0].GetIsValid(), "First editor must be valid.");
    Assert.IsFalse(list[1].GetIsValid(), "Second editor must be invalid.");
    Assert.IsFalse(list[2].GetIsValid(), "Third editor must be invalid.");
    Assert.IsFalse(list[3].GetIsValid(), "Fourth editor must be invalid.");
    Assert.IsFalse(list[0].focused, "First editor must not be focused.");
    Assert.IsFalse(list[1].focused, "Second editor must not be focused.");
    Assert.IsTrue(list[2].focused, "Third editor must be focused.");
    Assert.IsFalse(list[3].focused, "Fourth editor must not be focused.");
}