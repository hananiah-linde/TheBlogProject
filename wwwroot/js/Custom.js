let index = 0;

function AddTag() {
    var tagEntry = document.getElementById("TagEntry");

    let searchResult = search(tagEntry.value);

    if (searchResult != null) {

        Swal.fire({
            title: 'Error!',
            html: `<span class ='font-weight-bolder'>${searchResult.toUpperCase()}</span>`,
            icon: 'error'
        });

        // swalWithDarkButton.fire({
        //     html: `<span class ='font-weight-bolder'>${searchResult.toUpperCase()}</span>`
        // })

    }
    else {

        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagList").options[index++] = newOption;

    }

    tagEntry.value = "";
    return true;

}

function DeleteTag() {

    let tagCount = 1;
    let tagList = document.getElementById("TagList");

    if(!tagList) return false;

    if (tagList.selectedIndex == -1 ) {
        Swal.fire({
            title: 'Error!',
            html: `<span class ='font-weight-bolder'>NO TAG SELECTED</span>`,
            icon: 'error',
            timer: 3000
        });

        return true;
    }


    while (tagCount > 0) {

        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        }
        else {
            tagCount = 0;
        }

        index--;
    }
}

$("form").on("submit", function() {
    $("#TagList option").prop("selected", "selected");
})

if (tagValues != '') {
    let tagArray = tagValues.split(",");
    for (let loop = 0; loop < tagArray.length; loop++) {
        ReplaceTag(tagArray[loop], loop);
        index++;
    }
}

function ReplaceTag(tag, index) {
    let newOption = new Option(tag,tag);
    document.getElementById("TagList").options[index] = newOption;
}

function search(str) {

    if (str == "") {
        return 'Empty tags are not permitted';
    }

    var tagsElement = document.getElementById('TagList');

    if (tagsElement) {
        let options = tagsElement.options;

        for (let index = 0; index < options.length; index++) {

            if (options[index].value == str) {
                return `The tag #${str} was detected as a Duplicate and not permitted`;
            }
        }
    }
}

// const swalWithDarkButton = Swal.mixin({
//     customClass: {
//         confirmButton: 'btn btn-danger btn-md btn-outline-dark'
//     }
// });