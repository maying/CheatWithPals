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


    function queryBuilder(board) {
        var dataEntityName = "Word1";
        var query = "";
        var wildcard = "*";
        var anyOneLetter = " ";


        //first, checks if it isn't implemented yet
        if (!String.prototype.format) {
            String.prototype.format = function () {
                var args = arguments;
                return this.replace(/{(\d+)}/g, function (match, number) {
                    return typeof args[number] != 'undefined'
                  ? args[number]
                  : match
                    ;
                });
            };
        }

        var boardLetters = board.trim().split(" ");

        if (boardLetters.length == 1) {

            return RunOneCluster(board, boardLetters[0], wildcard, dataEntityName, anyOneLetter);
        }
        else {
            return query;
        }

    }

    function startsWith(str1, str2) {
        return str1.indexOf(str2) == 0;
    }

    function exclusiveIndexOf(str1, str2)
    {
        str1 = str1.toLowerCase();
        str2 = str2.toLowerCase();
        var i=0;
        for(i = 0; i<str1.length && startsWith(str1.substring(i), str2) ;++i)
        {
        }
        return i == str1.length ? -1 : i;
    }

    function RunOneCluster(board, boardLetter, wildcard, dataEntityName, anyOneLetter) {
        // Limited Max Space
        var query = "";
        if (board.indexOf(wildcard) === -1) {
            var maxLength = board.length;
            if (startsWith(board, anyOneLetter) && board.endsWith(anyOneLetter)) {
                // Use Case #1 anyOneLetter at the beginner AND end
                // i.e. _ _ A _ _ _ _                    
                query = "$filter=length({0}) le {1} and indexof({2},'{3}') le {4} and indexof({5},'{6}') gt 0".format
                    (
                    dataEntityName,
                    maxLength,
                    dataEntityName,
                    boardLetter,
                    board.indexOf(boardLetter),
                    dataEntityName,
                    boardLetter
                    );
            }
            else if (startsWith(board, anyOneLetter) && !board.endsWith(anyOneLetter)) {
                // Use case #2 
                // i.e. _ _ _ A
                query = "$filter=length({0}) le {1} and endswith({2}, '{3}')".format
                    (
                    dataEntityName,
                    maxLength,
                    dataEntityName,
                    boardLetter
                    );

            }
            else if (!startsWith(board, anyOneLetter) && board.endsWith(anyOneLetter)) {
                //  Use case #3 Start 
                // A _ _ _ _ _ (6)
                // A B _ _ _ _ (6)
                query = "$filter=length({0}) le {1} and startswith({2}, '{3}')".format
                (
                dataEntityName,
                maxLength,
                dataEntityName,
                boardLetter
                );
            }
        }
        else {
            if (startsWith(board, wildcard) && board.endsWith(anyOneLetter)) {
                /* Use case #4 
                a) Middle - Unlimited left
                * A _ _ 
                * A B _ 
                */
                // We do not support the case where the wildcard is in the middle of the board. 
            }
            else if(startsWith(board,anyOneLetter) && board.endsWith(wildcard)) {
                /* Use case #5
                b) Middle - unlimited right
                _ _ A *
                _ _ B A *
                */
                query = "$filter=indexof({0}, '{1}') eq {2}".format
                    (
                    dataEntityName,
                    boardLetter.replace(/\*/g, ""),
                    exclusiveIndexOf(board, anyOneLetter)
                    );
            }
            else if(startsWith(board,wildcard) && board.endsWith(wildcard))
            {
                /* Use case #6
                c) Middle - unlimited both directions
                * A *
                * A B *
                */

                query = "$filter=substringof('{0}', {1}) eq true".format
                    (
                    boardLetter.replace(/\*/g, ""),
                    dataEntityName
                    );
            }
            else {
                // We do not support the case where the wildcard is in the middle of the board. 
            }
        }
        return query;
    }

    function extractBoardLetter(board) {
        return board.replace(" ", "");
    }

    function getWords(board, hand, wordListObservable) {

        var query = queryBuilder(board);
        //var boardLetter = extractBoardLetter(board);
        var boardLetter = "A";

        return ajaxRequest("get", wordUrl() + '?boardLetter=' + boardLetter + '&letterOnHand=' + hand + "&" + query) // todo - add query to url
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