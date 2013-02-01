window.todoApp.todoListViewModel = (function (ko, datacontext) {
    /// <field name="todoLists" value="[new datacontext.todoList()]"></field>
    var todoLists = ko.observableArray(),
        error = ko.observable(),
        addTodoList = function () {
            var todoList = datacontext.createTodoList();
            todoList.isEditingListTitle(true);
            datacontext.saveNewTodoList(todoList)
                .then(addSucceeded)
                .fail(addFailed);

            function addSucceeded() {
                showTodoList(todoList);
            }
            function addFailed() {
                error("Save of new todoList failed");
            }
        },
        showTodoList = function (todoList) {
            todoLists.unshift(todoList); // Insert new todoList at the front
        },
        deleteTodoList = function (todoList) {
            todoLists.remove(todoList);
            datacontext.deleteTodoList(todoList)
                .fail(deleteFailed);

            function deleteFailed() {
                showTodoList(todoList); // re-show the restored list
            }
        },
        slotValues = [" ", "*", "{DL}", "{DW}", "{TL}", "{TW}", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"],
        slots = ko.observableArray([{ value: slotValues }]),
        wordList = ko.observableArray(),
        doSomething = function (formElement) {
            var length = formElement.elements.length;
            var board = "";
            var hand;
            for (i = 0; i < length; i++) {
                if (formElement[i].type == "select-one") {
                    board += formElement[i].value;
                } else if (formElement[i].type == "text") {
                    hand = formElement[i].value;
                }
            }          
            datacontext.getWords(board, hand, wordList);
        },
        addSlot = function () {
            
            slots.push({ value: slotValues });
        },
        getDef = function () {

            slots.push({ value: slotValues });
        };

    //datacontext.getTodoLists(todoLists, error); // load todoLists

    return {
        todoLists: todoLists,
        error: error,
        addTodoList: addTodoList,
        deleteTodoList: deleteTodoList,
        slots: slots,
        addSlot: addSlot,
        doSomething: doSomething,
        wordList: wordList,
        getDef:getDef
    };

})(ko, todoApp.datacontext);

// Initiate the Knockout bindings
ko.applyBindings(window.todoApp.todoListViewModel);
