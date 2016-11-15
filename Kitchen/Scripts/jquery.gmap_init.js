

$(function () {
    $("#map").gMap({
        markers: [{
            latitude: 42.885037, 
            longitude: 74.576301,
            html: 'Здесь могла быть ваша реклама...',
            icon: {
                image: '/Images/gmap_pin_mint.png',
                iconsize: [26, 46],
                iconanchor: [12, 46],
                infowindowanchor: [12, 0]
            }
        }],
        zoom: 15
    });


});

