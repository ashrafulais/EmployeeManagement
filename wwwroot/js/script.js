
function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpan = "#deletespan_" + uniqueId;
    var confirmDeleteSpan = "#confirmdeletespan_" + uniqueId;

    $(deleteSpan).toggle();
    $(confirmDeleteSpan).toggle();

    /*if (isDeleteClicked) {
        $(deleteSpan).hide();
        $(confirmDeleteSpan).show();
    } else {
        $('#' + deleteSpan).show();
        $('#' + confirmDeleteSpan).hide();
    }*/
}