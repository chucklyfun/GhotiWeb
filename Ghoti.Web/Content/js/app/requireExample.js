require.config({
    baseUrl: "/Scripts/",
    paths: {
        jquery: [
           'http://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min',
           'jquery-1.10.2.min'
        ],
    },
    shim: {
        jquery: { exports: '$' },
        'jquery.validate': ['jquery'],
        'bootstrap/dropdown': ['jquery'],
        'extras/jquery.pnotify': ['jquery'],
        
    }
});