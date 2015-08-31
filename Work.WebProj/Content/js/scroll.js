//test
$(window).scroll(function(){
     if ($(this).scrollTop() > 100) {
          $('.goTop').fadeIn();
     } else {
          $('.goTop').fadeOut();
     }
});
$(document).ready(function(){
     $('.goTop').click(function(){
          $("html, body").animate({ scrollTop: 0 }, 600);
          return false;
     });

     $('.scroll').click(function () {
         $('html, body').animate({
             scrollTop: $($.attr(this, 'href')).offset().top
         }, 600);
         return false;
     });
});
