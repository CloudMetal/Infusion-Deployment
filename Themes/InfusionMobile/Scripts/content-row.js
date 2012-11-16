$(document).ready(function() {
	var $pagesList = $('#pages-list'),
		$pagesDiv = $('#pages-list>li>div');
	$pagesDiv.slideUp();
	$pagesList.on('click', 'li.listed-page', function(e) {
		if(!$(this).children('div').hasClass('active')) {
			$pagesDiv.filter('.active').removeClass('active').slideUp();
			$(this).children('div').addClass('active').slideDown();
		}
	});
});