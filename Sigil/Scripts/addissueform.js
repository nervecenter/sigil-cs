$(function () {
    $('#orgBox').autocomplete({
        source: '@Url.Action("SearchOrgs_Cats", "Search")'
    });
});

function appendAddTextButton() {
    /*// Get the fieldset we'll be adding to
    var fieldset = document.getElementById("add-issue-fieldset");
    // Create a paragraph container for our button to confirm a new submission
    var newsubmissionp = document.createElement("p");
    newsubmissionp.id = "new-sub-p";
    // Append it
    fieldset.appendChild(newsubmissionp);
    // Create our button
    var newsubmissionbutton = document.createElement("input");
    newsubmissionbutton.id = "new-submission-button";
    newsubmissionbutton.type = "button";
    newsubmissionbutton.name = "newsubmissionbutton";
    newsubmissionbutton.value = "Add this as a new submission";
    newsubmissionbutton.onclick = function () { confirmNewSubmission() };
    // Get the paragraph we made earlier and append the button
    document.getElementById("new-sub-p").appendChild(newsubmissionbutton);
    document.getElementById("title-input").removeAttribute("oninput");*/

    // NEW VERSION
    var addissueform = document.getElementById("add-issue-form");
    var formgroup = document.createElement("div");
    formgroup.classList.add("form-group");
    var newpostbutton = document.createElement("a");
    newpostbutton.classList.add("btn btn-default");
    newpostbutton.innerHTML = "Add this as a new submission";
    newpostbutton.onclick = function () { confirmNewSubmission() };
    formgroup.appendChild(newpostbutton);
    addissueform.appendChild(formgroup);
}

function confirmNewSubmission() {
    // Get the fieldset we'll be adding to
    var fieldset = document.getElementById("add-issue-fieldset");
    // Remove the old button confirming a new submission
    document.getElementById("new-sub-p").remove();
    // Add our text field
    var textp = document.createElement("p");
    textp.id = "text-p";
    fieldset.appendChild(textp);
    var textlabel = document.createElement("label");
    textlabel.id = "text-label";
    textlabel.for = "text";
    textp.appendChild(textlabel);
    textlabel.innerHTML = "Explain your idea in-depth:";
    textp.appendChild(document.createElement("br"));
    var textinput = document.createElement("textarea");
    textinput.type = "text";
    textinput.name = "text";
    textinput.classList.add("add-issue-input");
    textinput.classList.add("add-issue-full-width");
    textinput.classList.add("add-issue-text-input");
    textinput.placeholder = "Explain your idea in-depth";
    textp.appendChild(textinput);
    // Add our submit button
    var submitp = document.createElement("p");
    submitp.id = "submit-p";
    fieldset.appendChild(submitp);
    var submitbutton = document.createElement("input");
    submitbutton.type = "submit";
    submitbutton.name = "buttonSubmit";
    submitbutton.value = "Add Issue";
    submitbutton.formAction = "@Url.Action("AddIssue", "Issue")";
    submitp.appendChild(submitbutton);
}