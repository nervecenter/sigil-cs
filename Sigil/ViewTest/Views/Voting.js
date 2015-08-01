function vote(votebutton, issueID) {
	var count = document.getElementById('count-' + issueID);
	if (votebutton.classList.contains('unchecked')) {
		// Call vote up action method by URL using jQuery
		votebutton.classList.remove('unchecked');
		votebutton.classList.add('checked');
		votebutton.src = "../Content/Images/check_mark_hover_small.png";
		count.innerHTML = parseInt(count.innerHTML, 10) + 1;
	} else if (votebutton.classList.contains('checked')) {
		// Call unvote up action method by URL using jQuery
		votebutton.classList.remove('checked');
		votebutton.classList.add('unchecked');
		votebutton.src = "../Content/Images/vote_circle_hover_small.png";
		count.innerHTML = parseInt(count.innerHTML, 10) - 1;
	}
}

function hover(votebutton) {
	if (votebutton.classList.contains('unchecked')) {
		votebutton.src = "../Content/Images/vote_circle_hover_small.png";		
	} else if (votebutton.classList.contains('checked')) {
		votebutton.src = "../Content/Images/check_mark_hover_small.png";
	}
}

function unhover(votebutton) {
	if (votebutton.classList.contains('unchecked')) {
		votebutton.src = "../Content/Images/vote_circle_small.png";
	} else if (votebutton.classList.contains('checked')) {
		votebutton.src = "../Content/Images/check_mark_small.png";
	}
}