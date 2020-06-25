$(function() {

  /******************/
  /* page preloader */
  /******************/
  $("#loader").delay(500).fadeOut(300);
  $(".mask").delay(800).fadeOut(300);


  if (Modernizr.mq("screen and (max-width:768px)")) {
    jQuery("body").toggleClass("body");
  } else {
    var s = skrollr.init({
      mobileDeceleration: 1,
      mobileCheck: function() {
        //hack - forces mobile version to be off
        return false;
      },
      edgeStrategy: 'set',
      forceHeight: false,
      smoothScrolling: true,
      smoothScrollingDuration: 300,
      easing: {
        WTF: Math.random,
        inverted: function(p) {
          return 1-p;
        }
      }
    });
  }

  var widthLess768 = function() {
    if (Modernizr.mq("screen and (max-width:768px)")) {
      $('.navbar .dropdown[data-init=onHover]').attr('data-init', 'onClick');
      skrollr.init().destroy();
    } else {
      $('.navbar .dropdown[data-init=onClick]').attr('data-init', 'onHover');
      $('.navbar .nav .search input').on('blur', function () {
        $(this).val('');
      });
      skrollr.init()
    }
  };

  widthLess768();

  $(".rotate-words").textrotator({
    animation: "flipUp", // You can pick the way it animates when rotating through words. Options are dissolve (default), fade, flip, flipUp, flipCube, flipCubeUp and spin.
    separator: ",", // If you don't want commas to be the separator, you can define a new separator (|, &, * etc.) by yourself using this field.
    speed: 4000 // How many milliseconds until the next word show.
  });

  var projectsCarousel = $("#latest-projects .owl-carousel");

  projectsCarousel.owlCarousel({
    items: 3,
    itemsDesktop: false,
    pagination: false
  });

  // Custom Navigation Events
  $("#latest-projects .custom-navigation .next").click(function(){
    projectsCarousel.trigger('owl.next');
  })
  $("#latest-projects .custom-navigation .prev").click(function(){
    projectsCarousel.trigger('owl.prev');
  })

  $('a[data-nivobox^="true"]').nivoLightbox({
    effect: 'fade'
  });

  $('.twitter-feed').tweet({
    username: 'envato', //twitter username 
    modpath: './assets/js/vendor/tweet-js/',
    join_text: '',
    avatar_size: null,
    count: 10, //number of tweets
    auto_join_text_default: ' ',
    loading_text: 'loading latest tweets...' //text displayed while loading tweets
  });

  var clientsCarousel = $("#our-clients .owl-carousel");

  clientsCarousel.owlCarousel({
    items: 4,
    pagination: false
  });

  // Custom Navigation Events
  $("#our-clients .custom-navigation .next").click(function(){
    clientsCarousel.trigger('owl.next');
  })
  $("#our-clients .custom-navigation .prev").click(function(){
    clientsCarousel.trigger('owl.prev');
  })

  var clientsFeedbackCarousel = $("#clients-feedback .owl-carousel");

  clientsFeedbackCarousel.owlCarousel({
    singleItem: true,
    pagination: false
  });

  // Custom Navigation Events
  $("#clients-feedback .custom-navigation .next").click(function(){
    clientsFeedbackCarousel.trigger('owl.next');
  })
  $("#clients-feedback .custom-navigation .prev").click(function(){
    clientsFeedbackCarousel.trigger('owl.prev');
  })

  // Main Slider
  var time = 7; // time in seconds
 
  var $progressBar,
      $bar, 
      isPause, 
      tick,
      percentTime;


  var slider = $("#slider .owl-carousel");

  slider.owlCarousel({
    singleItem: true,
    stopOnHover: true,
    transitionStyle : "fade",
    addClassActive: true,
    afterMove:  moved,
    afterInit:  progressBar,
    startDragging : pauseOnDragging
  });
 
  //Init progressBar
  function progressBar(){
    //build progress bar elements
    buildProgressBar();
    //start counting
    start();
    sliderAnimations();
  };

  //create div#progressBar and div#bar then prepend to $("#owl-demo")
  function buildProgressBar(){
    $progressBar = $("<div>",{
      id:"progressBar"
    });
    $bar = $("<div>",{
      id:"bar"
    });
    $progressBar.append($bar).prependTo(slider);
  };

  function start() {
    //reset timer
    percentTime = 0;
    isPause = false;
    //run interval every 0.01 second
    tick = setInterval(interval, 10);
  };

  function interval() {
    if(isPause === false){
      percentTime += 1 / time;
      $bar.css({
         width: percentTime+"%"
       });
      //if percentTime is equal or greater than 100
      if(percentTime >= 100){
        //slide to next item 
        slider.trigger('owl.next')
      }
    }
  };

  //pause while dragging 
  function pauseOnDragging(){
    isPause = true;
  };

  //moved callback
  function moved(){
    //clear interval
    clearTimeout(tick);
    //start again
    start();
    sliderAnimations();
  };

  //uncomment this to make pause on mouseover 
  slider.on('mouseover',function(){
    isPause = true;
  })
  slider.on('mouseout',function(){
    isPause = false;
  });

  // Custom Navigation Events
  $("#slider .navigation .next").click(function(){
    slider.trigger('owl.next');
  })
  $("#slider .navigation .prev").click(function(){
    slider.trigger('owl.prev');
  })

  function sliderAnimations(){

    $('#slider .owl-item').not('active').find('.caption').each(function(){
      var caption = $(this),
          captionAnimation = caption.data('animation');

      caption.removeClass('animated' + ' ' + captionAnimation);
    });

    $('#slider .owl-item.active .caption').each(function(){
      var caption = $(this),
          captionAnimation = caption.data('animation'),
          captionDelay = 0;

      captionDelay = caption.data('delay');

      window.setTimeout(function(){
        caption.addClass('animated' + ' ' + captionAnimation);
      }, captionDelay);
    })
    
  }


  // FLICKR FEED

  $('.flickr-feed').append('<ul class="photo-stream"></ul>');

  $('.flickr-feed ul').jflickrfeed({
      limit: 12,
      qstrings: {
        id: '12625534@N00' // enter Flickr ID          
      },

      itemTemplate: '<li><a href="{{image_b}}" title="{{title}}" data-nivobox="true" data-lightbox-gallery="photo-stream"><img src="{{image_s}}" alt="{{title}}" /></a><span class="photo-stream-overlay"><i class="fa fa-arrows-alt"></i></span></li>'

  }, function (data) {
      $('.flickr-feed li a').nivoLightbox({
          effect: 'fade'
      });
  });

  var initTweetFeed = function() {
    !function(d,s,id){var js,fjs=d.getElementsByTagName(s)[0],p=/^http:/.test(d.location)?'http':'https';if(!d.getElementById(id)){js=d.createElement(s);js.id=id;js.src=p+"://platform.twitter.com/widgets.js";fjs.parentNode.insertBefore(js,fjs);}}(document,"script","twitter-wjs");
  }

  initTweetFeed();

  //Animated Progress Bars
  $('.progress').each(function () {
    $(this).appear(function() {
      $(this).find('.progress-bar').css({
        width: $(this).find('.progress-bar').data('percent')
      });
    });
  });

  //Elements Appearing
  $('.appear-el').each(function () {
    $(this).appear(function() {
      var animation = $(this).data('animation');

      $(this).delay(150).queue(function(next) {
        $(this).addClass('animated' + ' ' + animation);
        next();
      });
    });
  });

  // navbar dropdown on hover

  $('.navbar .dropdown').hover(function() {
    if ($(this).attr('data-init') == 'onHover') {
      $(this).find('.dropdown-menu').first().stop(true, true).slideToggle(200);
      $(this).addClass('open');
    }
  }, function() {
    if ($(this).attr('data-init') == 'onHover') {
      $(this).find('.dropdown-menu').first().stop(true, true).slideToggle(200);
      $(this).removeClass('open');
    }
  });

  $('.navbar .dropdown').not('a').click(function(e){
    if ($(e.target).hasClass('dropdown-toggle')) {
      if ($(this).attr('data-init') == 'onHover') {
        return false;
      }
    }
  });

  $(window).resize(function() {
    widthLess768();
  });

  // scroll to top

  $(window).scroll(function(){
    if ($(this).scrollTop() > 200) {
      $('.scroll-top').fadeIn('slow');
    } else {
      $('.scroll-top').fadeOut('slow');
    }
  });

  $('.scroll-top').click(function () {
    $('body,html').animate({
      scrollTop: 0
    }, 800);
    return false;
  });


})

// Parallax  
$(window).bind('load', function () {
  parallaxInit();             
});

function parallaxInit() {
  $('#twitter-parallax').parallax("50%", 0.1);
}
