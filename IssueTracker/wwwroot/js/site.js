// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function updateIssueStatus(event) {
    let issueId = $(event.target).attr("issueId");
    let status = $(event.target).find("option:selected").val();
    $(event.target).removeClass("bg-info bg-secondary bg-success");
    $(event.target).attr("disabled", true).addClass("bg-dark");
    $(event.target).find("option:selected").text(". . .");

    $.ajax({
        type: 'POST',
        url: "/Issue/UpdateStatus",
        data: {
            id: issueId,
            status: status
        },
        success: function (result) {
            switch (result) {
                case "To Do":
                    $(event.target).removeClass("bg-dark");
                    $(event.target).addClass("bg-success");
                    break;
                case "In Progress":
                    $(event.target).removeClass("bg-dark");
                    $(event.target).addClass("bg-info");
                    break;
                case "Done":
                    $(event.target).removeClass("bg-dark");
                    $(event.target).addClass("bg-secondary");
                    break;
            }
            $(event.target).attr("disabled", false);
            $(event.target).find("option:selected").text(status);
            console.log(['result', result]);
        }
    });
}

function updateIssuePriority(event) {
    let issueId = $(event.target).attr("issueId");
    let priority = $(event.target).find("option:selected").val();

    $(event.target).attr("disabled", true).addClass("bg-dark text-white");
    $(event.target).find("option:selected").text(". . .");

    $.ajax({
        type: 'POST',
        url: "/Issue/UpdatePriority",
        data: {
            id: issueId,
            priority: priority
        },
        success: function (result) {
            console.log(['result', result]);
            $(event.target).removeClass("bg-dark text-white");
            $(event.target).attr("disabled", false);
            $(event.target).find("option:selected").text(priority);
        }
    });
}

function updateIssueType(event) {
    let issueId = $(event.target).attr("issueId");
    let type = $(event.target).find("option:selected").val();

    $(event.target).attr("disabled", true).addClass("bg-dark text-white");
    $(event.target).find("option:selected").text(". . .");

    $.ajax({
        type: 'POST',
        url: "/Issue/UpdateType",
        data: {
            id: issueId,
            type: type
        },
        success: function (result) {
            console.log(['result', result]);
            $(event.target).removeClass("bg-dark text-white");
            $(event.target).attr("disabled", false);
            $(event.target).find("option:selected").text(type);
        }
    });
}

function updateIssueAssignedUser(event) {
    let issueId = $(event.target).attr("issueId");
    let userId = $(event.target).find("option:selected").val();

    $(event.target).attr("disabled", true).addClass("bg-dark text-white");
    $(event.target).find("option:selected").text(". . .");

    $.ajax({
        type: 'POST',
        url: "/Issue/UpdateAssignedUser",
        data: {
            id: issueId,
            userId: userId
        },
        success: function (result) {
            console.log(['result', result]);
            $(event.target).removeClass("bg-dark text-white");
            $(event.target).attr("disabled", false);
            $(event.target).find("option:selected").text(result);
        }
    });
}

function sortIssues(event) {
    $.ajax({
        type: "GET",
        url: "Issue/IssueTable",
        data: {
            sortField: $(event.target).attr("field")
        },
        success: function (result) {
            $(event.currentTarget).html(result);
        }
    });
}

$(document).ready(function () {
    $('.edit-issue-container').change(function (event) {
        switch ($(event.target).attr("tag")) {
            case "status-dropdown":
                updateIssueStatus(event);
                break;
            case "priority-dropdown":
                updateIssuePriority(event);
                break;
            case "type-dropdown":
                updateIssueType(event);
                break;
            case "assigned-dropdown":
                updateIssueAssignedUser(event);
        }
    }).click(function (event) {

        //if (event.target.matches('.navMenu-button') ||
        //    event.target.parentNode.matches('.navMenu-button')
        //)
        switch ($(event.target).attr("tag")) {
            case "btn-edit-title":
                console.log("title edit clicked");
                break;
            case "btn-edit-desc":
                console.log("description edit clicked");
                break;
        }
    });

    $('.issue-list-container').change(function (event) {
        switch ($(event.target).attr("tag")) {
            case "status-dropdown":
                updateIssueStatus(event);
                break;
            case "priority-dropdown":
                updateIssuePriority(event);
                break;
            case "type-dropdown":
                updateIssueType(event);
                break;
        }
    }).click(function (event) {
        if ($(event.target).attr("tag") == "sort-button") {
            sortIssues(event);
        }

        let element = event.target.parentElement;
        if ($(element).attr("tag") == "issue-row") {
            let issueId = $(element).attr("issueId");
            console.log(["issueId", issueId]);
            location.href = "/Issue/Edit/" + issueId;
        }
    });
});

