// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function () {




    /* Admin Index Page */
    $(".user-list-container").click(function (event) {

        let element = event.target.parentElement;
        if ($(element).attr("tag") == "user-row") {
            let userId = $(element).attr("userId");

            location.href = "/Administration/EditUserRole?userId=" + userId;
        }
    });



    /* ProjectTableViewComponent */

    $(".project-list-container").click(function (event) {
        switch ($(event.target).attr("tag")) {
            case "delete-project-btn":
                // Show project delete confirmation modal
                $("#delete-project-title").text($(event.target).attr("projectTitle"));
                $("#confirm-modal-btn").attr("projectId", $(event.target).attr("projectId"));
                $(".delete-project-modal").modal("show");                
                break;
            case "close-modal-btn":
                // Hide project delete modal
                $(".delete-project-modal").modal("hide");
                break;
            case "confirm-modal-btn":
                // Delete project and hide modal
                console.log("delete project", $(event.target).attr("projectId"));
                $.ajax({
                    type: "POST",
                    url: "Project/Delete",
                    data: {
                        pid: $(event.target).attr("projectId")
                    },
                    success: function (result) {
                        $("#" + $(event.target).attr("projectId")).attr("hidden", true);
                    }
                });
                $(".delete-project-modal").modal("hide");
                break;
        }
    });

    /* Edit Project Page */

    $(".edit-project-container").click(function (event) {
        switch ($(event.target).attr("tag")) {
            case "btn-edit-title":
                // Show title edit div
                $(".display-title-container").attr("hidden", true);
                $(".edit-title-container").attr("hidden", false);
                break;
            case "edit-title-submit":
                // Update title and hide edit title div
                $.ajax({
                    type: 'POST',
                    url: "/Project/UpdateTitle",
                    data: {
                        pid: $(".edit-title-input").attr("projectId"),
                        title: $(".edit-title-input").val()
                    },
                    success: function (result) {
                        console.log(['result', result]);
                        $(".display-title-container").attr("hidden", false);
                        $(".edit-title-container").attr("hidden", true);
                        $(".display-title").text(result);
                    }
                });
                break;
            case "edit-title-cancel":
                // Hide edit title div
                $(".display-title-container").attr("hidden", false);
                $(".edit-title-container").attr("hidden", true);
                break;
            case "btn-edit-desc":
                // Show edit description div
                $(".display-desc-container").attr("hidden", true);
                $(".edit-desc-container").attr("hidden", false);
                break;
            case "edit-desc-submit":
                // Update description and hide edit desc div
                $.ajax({
                    type: 'POST',
                    url: "/Project/UpdateDesc",
                    data: {
                        pid: $(".edit-desc-input").attr("projectId"),
                        desc: $(".edit-desc-input").val()
                    },
                    success: function (result) {
                        console.log(['result', result]);
                        $(".display-desc-container").attr("hidden", false);
                        $(".edit-desc-container").attr("hidden", true);
                        $(".display-desc").text(result);
                    }
                });
                break;
            case "edit-desc-cancel":
                // Hide edit description div
                $(".display-desc-container").attr("hidden", false);
                $(".edit-desc-container").attr("hidden", true);
                break;
            case "remove-users-btn":
                $("#remove-users-modal").modal("show");
                break;
            case "confirm-remove-btn":
                $.ajax({
                    type: 'POST',
                    url: "/Project/RemoveUsersFromProject",
                    data: {
                        pid: $(event.target).attr("projectId"),
                        userIds: $(".user-checkbox:checked").map(function () {
                                    return $(this).attr("userId");
                                }).get()
                    },
                    success: function (result) {
                        refreshUserTables($(event.target).attr("projectId"));
                        $("#remove-users-modal").modal("hide");
                    }
                });
                break;
            case "cancel-remove-btn":
                $("#remove-users-modal").modal("hide");
                break;
            case "add-users-btn":
                $("#add-users-modal").modal("show");
                break;
            case "confirm-add-btn":
                $.ajax({
                    type: 'POST',
                    url: "/Project/AddUsersToProject",
                    data: {
                        pid: $(event.target).attr("projectId"),
                        userIds: $(".user-checkbox:checked").map(function () {
                            return $(this).attr("userId");
                        }).get()
                    },
                    success: function (result) {
                        refreshUserTables($(event.target).attr("projectId"));
                        $("#add-users-modal").modal("hide");
                    }
                });
            case "cancel-add-btn":
                $("#add-users-modal").modal("hide");
                break;
            case "submit-proj-owner-btn":
                $.ajax({
                    type: 'POST',
                    url: "/Project/UpdateProjectOwner",
                    data: {
                        pid: $(event.target).attr("projectId"),
                        userId: $("#project-manager-dropdown option:selected").val()
                    },
                    success: function (result) {
                    }
                });
        }
    });

    function refreshUserTables(projectId) {
        $.ajax({
            type: 'GET',
            url: "/Administration/UserTable",
            data: {
                projectId: projectId,
                filter: "IN PROJECT"
            },
            success: function (result) {
                $("#project-members-table").html(result);
            }
        });
        $.ajax({
            type: 'GET',
            url: "/Administration/UserTable",
            data: {
                projectId: projectId,
                filter: "IN PROJECT",
                isSelectable: true
            },
            success: function (result) {
                $("#remove-users-table").html(result);
            }
        });
        $.ajax({
            type: 'GET',
            url: "/Administration/UserTable",
            data: {
                projectId: projectId,
                filter: "NOT IN PROJECT",
                isSelectable: true
            },
            success: function (result) {
                $("#add-users-table").html(result);
            }
        });
    }

    /* Edit Issue Page */

    $(".edit-issue-container").change(function (event) {
        switch ($(event.target).attr("tag")) {
            case "status-dropdown":
                // Update Issue Status
                let status = $(event.target).find("option:selected").val();
                $(event.target).removeClass("bg-info bg-secondary bg-success");
                $(event.target).attr("disabled", true).addClass("bg-dark");
                $(event.target).find("option:selected").text(". . .");

                $.ajax({
                    type: 'POST',
                    url: "/Issue/UpdateStatus",
                    data: {
                        id: $(event.target).attr("issueId"),
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
                        refreshHistory($(event.target).attr("issueId"));
                    }
                });
                break;
            case "priority-dropdown":
                // Update Issue Priority
                let priority = $(event.target).find("option:selected").val();

                $(event.target).attr("disabled", true).addClass("bg-dark text-white");
                $(event.target).find("option:selected").text(". . .");

                $.ajax({
                    type: 'POST',
                    url: "/Issue/UpdatePriority",
                    data: {
                        id: $(event.target).attr("issueId"),
                        priority: priority
                    },
                    success: function (result) {
                        console.log(['result', result]);
                        $(event.target).removeClass("bg-dark text-white");
                        $(event.target).attr("disabled", false);
                        $(event.target).find("option:selected").text(priority);
                        refreshHistory($(event.target).attr("issueId"));
                    }
                });
                break;
            case "type-dropdown":
                // Update Issue Type
                let type = $(event.target).find("option:selected").val();
                $(event.target).attr("disabled", true).addClass("bg-dark text-white");
                $(event.target).find("option:selected").text(". . .");

                $.ajax({
                    type: 'POST',
                    url: "/Issue/UpdateType",
                    data: {
                        id: $(event.target).attr("issueId"),
                        type: type
                    },
                    success: function (result) {
                        $(event.target).removeClass("bg-dark text-white").attr("disabled", false);
                        $(event.target).find("option:selected").text(result);
                        refreshHistory($(event.target).attr("issueId"));
                    }
                });
                break;
            case "assigned-dropdown":
                // Update Assigned User
                $(event.target).attr("disabled", true).addClass("bg-dark text-white");
                $(event.target).find("option:selected").text(". . .");

                $.ajax({
                    type: 'POST',
                    url: "/Issue/UpdateAssignedUser",
                    data: {
                        id: $(event.target).attr("issueId"),
                        userId: $(event.target).find("option:selected").val()
                    },
                    success: function (result) {
                        $(event.target).removeClass("bg-dark text-white").attr("disabled", false).find("option:selected").text(result);
                        refreshHistory($(event.target).attr("issueId"));
                    }
                });
        }
    }).click(function (event) {
        switch ($(event.target).attr("tag")) {
            case "btn-edit-title":
                // Show Edit Title Div
                $(".display-title-container").attr("hidden", true);
                $(".edit-title-container").attr("hidden", false);
                break;
            case "edit-title-submit":
                // Update Issue Title and Hide Div
                $.ajax({
                    type: 'POST',
                    url: "/Issue/UpdateTitle",
                    data: {
                        id: $(".edit-title-input").attr("issueId"),
                        title: $(".edit-title-input").val()
                    },
                    success: function (result) {
                        console.log(['result', result]);
                        $(".display-title-container").attr("hidden", false);
                        $(".edit-title-container").attr("hidden", true);
                        $(".display-title").text(result);
                        refreshHistory($(".edit-title-input").attr("issueId"));
                    }
                });
                break;
            case "edit-title-cancel":
                // Hide Edit Title Input
                $(".display-title-container").attr("hidden", false);
                $(".edit-title-container").attr("hidden", true);
                break;
            case "btn-edit-desc":
                // Show Edit Desc Div
                $(".display-desc-container").attr("hidden", true);
                $(".edit-desc-container").attr("hidden", false);
                break;
            case "edit-desc-submit":
                // Update Issue Description and Hide Edit Div
                $.ajax({
                    type: 'POST',
                    url: "/Issue/UpdateDesc",
                    data: {
                        id: $(".edit-desc-input").attr("issueId"),
                        desc: $(".edit-desc-input").val()
                    },
                    success: function (result) {
                        $(".display-desc-container").attr("hidden", false);
                        $(".edit-desc-container").attr("hidden", true);
                        $(".display-desc").text(result);
                        refreshHistory($(".edit-desc-input").attr("issueId"));
                    }
                });
                break;
            case "edit-desc-cancel":
                // Hide Edit Desc Div
                $(".display-desc-container").attr("hidden", false);
                $(".edit-desc-container").attr("hidden", true);
                break;
            case "delete-issue-modal-btn":
                // Show Delete Issue confirmation modal
                $("#delete-issue-modal").modal("show");
                break;
            case "close-modal-btn":
                // Hide Delete issue confirmation modal
                $("#delete-issue-modal").modal("hide");
                break;
            case "submit-comment-btn":
                $.ajax({
                    type: "POST",
                    url: "/Issue/CreateIssueComment",
                    data: {
                        id: $(event.target).attr("issueId"),
                        text: $("#comment-input").val()
                    },
                    success: function (result) {
                        $("#comment-input").val("");
                        refreshComments($(event.target).attr("issueId"));
                    }
                });
                break;
            case "delete-comment-btn":
                let commentId = $(event.target).attr("commentId");
                $("#delete-comment-modal").modal("show");
                $("#confirm-delete-comment-btn").attr("commentId", commentId);
                break;
            case "cancel-delete-comment-btn":
                $("#delete-comment-modal").modal("hide");
                break;
            case "confirm-delete-comment-btn":
                $(event.target).attr("commentId");
                $.ajax({
                    type: "POST",
                    url: "/Issue/DeleteIssueComment",
                    data: {
                        cid: $(event.target).attr("commentId")
                    },
                    success: function (result) {
                        refreshComments($(event.target).attr("issueId"));
                        
                    }
                })
                $("#delete-comment-modal").modal("hide");
                break;
        }
    });

    function refreshHistory(issueId) {
        $.ajax({
            type: "GET",
            url: "/Issue/IssueHistory",
            data: {
                id: issueId
            },
            success: function (result) {
                $("#history-container").html(result);
            }
        })
    }

    function refreshComments(issueId) {
        $.ajax({
            type: "GET",
            url: "/Issue/IssueCommentList",
            data: {
                id: issueId
            },
            success: function (result) {
                $("#comment-container").html(result);
            }
        })
    }


    /* Issue Index Page */

    $(".issue-filter-dropdown").change(function (event) {
        // Filter IssueTableViewComponent
        let filter = $(event.target).find("option:selected").val();

        $.ajax({
            type: "GET",
            url: "Issue/IssueTable",
            data: {
                filter: filter,
                pageIndex: 1
            },
            success: function (result) {
                $(".issue-list-container").html(result);
            }
        });
    });

    $("#issue-search-btn").click(function (event) {
        // Search IssueTableViewComponent
        let searchString = $("#issue-search-input").val();
        let emptySearch = false;
        if (searchString === "") {
            emptySearch = true;
        }
        console.log(searchString);
        $.ajax({
            type: "GET",
            url: "Issue/IssueTable",
            data: {
                searchString: searchString,
                emptySearch: emptySearch
            },
            success: function (result) {
                $(".issue-list-container").html(result);
            }
        });
    });



    /* IssueTableViewComponent */

    $('.issue-list-container').change(function (event) {
        switch ($(event.target).attr("tag")) {
            case "status-dropdown":
                // Update Issue Status
                let status = $(event.target).find("option:selected").val();
                $(event.target).removeClass("bg-info bg-secondary bg-success");
                $(event.target).attr("disabled", true).addClass("bg-dark");
                $(event.target).find("option:selected").text(". . .");

                $.ajax({
                    type: 'POST',
                    url: "/Issue/UpdateStatus",
                    data: {
                        id: $(event.target).attr("issueId"),
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
                    }
                });
                break;
            case "priority-dropdown":
                // Update Issue Priority
                let priority = $(event.target).find("option:selected").val();

                $(event.target).attr("disabled", true).addClass("bg-dark text-white");
                $(event.target).find("option:selected").text(". . .");

                $.ajax({
                    type: 'POST',
                    url: "/Issue/UpdatePriority",
                    data: {
                        id: $(event.target).attr("issueId"),
                        priority: priority
                    },
                    success: function (result) {
                        console.log(['result', result]);
                        $(event.target).removeClass("bg-dark text-white");
                        $(event.target).attr("disabled", false);
                        $(event.target).find("option:selected").text(priority);
                    }
                });
                
                break;
            case "type-dropdown":
                // Update Issue Type
                let type = $(event.target).find("option:selected").val();

                $(event.target).attr("disabled", true).addClass("bg-dark text-white");
                $(event.target).find("option:selected").text(". . .");

                $.ajax({
                    type: 'POST',
                    url: "/Issue/UpdateType",
                    data: {
                        id: $(event.target).attr("issueId"),
                        type: type
                    },
                    success: function (result) {
                        $(event.target).removeClass("bg-dark text-white").attr("disabled", false);
                        $(event.target).find("option:selected").text(type);
                    }
                });
                break;
        }
    }).click(function (event) {
        switch ($(event.target).attr("tag")) {
            case "sort-button":
                // Sort Issues
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
                break;
            case "next-page-btn":
                // Get Next Page
                $.ajax({
                    type: "GET",
                    url: "Issue/IssueTable",
                    data: {
                        pageIndex: Number($(event.target).attr("pageIndex")) + 1
                    },
                    success: function (result) {
                        $(".issue-list-container").html(result);
                    }
                });
                break;
            case "prev-page-btn":
                // Get Prev Page
                $.ajax({
                    type: "GET",
                    url: "Issue/IssueTable",
                    data: {
                        pageIndex: Number($(event.target).attr("pageIndex")) - 1
                    },
                    success: function (result) {
                        $(".issue-list-container").html(result);
                    }
                });
                break;
        }

        // Table Row Click Event
        let element = event.target.parentElement;
        if ($(element).attr("tag") == "issue-row") {
            let issueId = $(element).attr("issueId");
            console.log(["issueId", issueId]);
            location.href = "/Issue/Edit/" + issueId;
        }
    });
});

