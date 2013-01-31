window.todoApp = window.todoApp || {};

window.todoApp.datacontext = (function () {

    var datacontext = {
        getTodoLists: getTodoLists,
        createTodoItem: createTodoItem,
        createTodoList: createTodoList,
        saveNewTodoItem: saveNewTodoItem,
        saveNewTodoList: saveNewTodoList,
        saveChangedTodoItem: saveChangedTodoItem,
        saveChangedTodoList: saveChangedTodoList,
        deleteTodoItem: deleteTodoItem,
        deleteTodoList: deleteTodoList,
        getWords: getWords
    };

    return datacontext;

    function getWords(board, hand, wordListObservable) {
        // todo - insert query builder lgoci
        // var query = QueryBuilder(...);
        var query = '';
        var boardLetter = board; // remove all other space/bonus/wildcard
        

        return ajaxRequest("get", wordUrl() + '?boardLetter=' + boardLetter + '&letterOnHand=' + hand) // todo - add query to url
            .done(getSucceeded)
            .fail(getFailed);

        function getSucceeded(data) {
            wordListObservable.removeAll();
            for (i = 0; i < data.length; i++) {
                wordListObservable.push(data[i]);
            }
        }

        function getFailed() {
            alert('failed');
            wordListObservable("Error retrieving words lists.");
        }
    }

    function getTodoLists(todoListsObservable, errorObservable) {
        return ajaxRequest("get", todoListUrl())
            .done(getSucceeded)
            .fail(getFailed);

        function getSucceeded(data) {
            var mappedTodoLists = $.map(data, function (list) { return new createTodoList(list); });
            todoListsObservable(mappedTodoLists);
        }

        function getFailed() {
            errorObservable("Error retrieving todo lists.");
        }
    }
    function createTodoItem(data) {
        return new datacontext.todoItem(data); // todoItem is injected by todo.model.js
    }
    function createTodoList(data) {
        return new datacontext.todoList(data); // todoList is injected by todo.model.js
    }
    function saveNewTodoItem(todoItem) {
        clearErrorMessage(todoItem);
        return ajaxRequest("post", todoItemUrl(), todoItem)
            .done(function (result) {
                todoItem.todoItemId = result.todoItemId;
            })
            .fail(function () {
                todoItem.errorMessage("Error adding a new todo item.");
            });
    }
    function saveNewTodoList(todoList) {
        clearErrorMessage(todoList);
        return ajaxRequest("post", todoListUrl(), todoList)
            .done(function (result) {
                todoList.todoListId = result.todoListId;
                todoList.userId = result.userId;
            })
            .fail(function () {
                todoList.errorMessage("Error adding a new todo list.");
            });
    }
    function deleteTodoItem(todoItem) {
        return ajaxRequest("delete", todoItemUrl(todoItem.todoItemId))
            .fail(function () {
                todoItem.errorMessage("Error removing todo item.");
            });
    }
    function deleteTodoList(todoList) {
        return ajaxRequest("delete", todoListUrl(todoList.todoListId))
            .fail(function () {
                todoList.errorMessage("Error removing todo list.");
            });
    }
    function saveChangedTodoItem(todoItem) {
        clearErrorMessage(todoItem);
        return ajaxRequest("put", todoItemUrl(todoItem.todoItemId), todoItem)
            .fail(function () {
                todoItem.errorMessage("Error updating todo item.");
            });
    }
    function saveChangedTodoList(todoList) {
        clearErrorMessage(todoList);
        return ajaxRequest("put", todoListUrl(todoList.todoListId), todoList)
            .fail(function () {
                todoList.errorMessage("Error updating the todo list title. Please make sure it is non-empty.");
            });
    }

    // Private
    function clearErrorMessage(entity) { entity.errorMessage(null); }
    function ajaxRequest(type, url, data) { // Ajax helper
        var options = {
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: type,
            data: data ? data.toJson() : null
        };
        var antiForgeryToken = $("#antiForgeryToken").val();
        if (antiForgeryToken) {
            options.headers = {
                'RequestVerificationToken': antiForgeryToken
            }
        }
        return $.ajax(url, options);
    }
    // routes
    function wordUrl(id) { return "/api/word/" + (id || ""); }
    function todoListUrl(id) { return "/api/todolist/" + (id || ""); }
    function todoItemUrl(id) { return "/api/todo/" + (id || ""); }

})();