/*! SmartAdmin - v1.5 - 2014-09-27 */ ! function(a) {
    a.fn.SuperBox = function () {
        console.debug("SuperBox called");
        var b = a('<div class="superbox-show"></div>'),
            c = a('<img src="" class="superbox-current-img"><div id="imgInfoBox" class="superbox-imageinfo inline-block"> <h1>Image Title</h1><span><p><em>http://imagelink.com/thisimage.jpg</em></p><p class="superbox-img-description">Image description</p><p><a href="javascript:void(0);" class="btn btn-primary btn-sm">Edit Picture</a> <a href="javascript:void(0);" class="btn btn-danger btn-sm">Delete</a></p></span> </div>'),
            d = a('<div class="superbox-close txt-color-white"><i class="fa fa-times fa-lg"></i></div>');
        b.append(c).append(d);
        a(".superbox-imageinfo");
        return this.each(function() {
            a(".superbox-list").click(function () {
                $this = a(this);
                var d = $this.find(".superbox-img"),
                    e = d.data("img"),
                    f = d.attr("alt") || "No description",
                    g = e,
                    h = d.attr("title") || "No Title";
                c.attr("src", e), a(".superbox-list").removeClass("active"), $this.addClass("active"), c.find("em").text(g), c.find(">:first-child").text(h), c.find(".superbox-img-description").text(f), 0 == a(".superbox-current-img").css("opacity") && a(".superbox-current-img").animate({
                    "opacity": 1
                }), a(this).next().hasClass("superbox-show") ? (a(".superbox-list").removeClass("active"), b.toggle()) : (b.insertAfter(this).css("display", "block"), $this.addClass("active")), a("html, body").animate({
                    "scrollTop": b.position().top - d.width()
                }, "medium")
            }), a(".superbox").on("click", ".superbox-close", function() {
                a(".superbox-list").removeClass("active"), a(".superbox-current-img").animate({
                    "opacity": 0
                }, 200, function() {
                    a(".superbox-show").slideUp()
                })
            })
        })
    }
}(jQuery);