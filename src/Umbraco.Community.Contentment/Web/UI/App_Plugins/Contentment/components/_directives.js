/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco.directives.html").directive("lkHtmlAttributes", [
    function () {
        return {
            restrict: "A",
            scope: {
                attributes: "&lkHtmlAttributes"
            },
            link: function (scope, element, attrs) {
                var attributes = scope.attributes();
                if (Array.isArray(attributes) && attributes.length > 0) {
                    attributes.forEach(function (x) {
                        if (x.name === "class") {
                            // NOTE: Slight bug, it did not account for existing class values.
                            element.addClass(x.value);
                        } else {
                            element.attr(x.name, x.value);
                        }
                    });
                }
            }
        };
    }
]);

angular.module("umbraco.directives.html").directive("lkBindHtmlTemplate", [
    "$compile",
    function ($compile) {
        return {
            restrict: "A",
            replace: true,
            link: function (scope, element, attrs) {
                scope.$watch(
                    function (scope) {
                        return scope.$eval(attrs.lkBindHtmlTemplate);
                    },
                    function (value) {
                        element.html(value);
                        $compile(element.contents())(scope);
                    }
                );
            }
        };
    }
]);

angular.module("umbraco.directives").component("leeWasHere", {
    template: "<img ng-src=\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAKAAAAAyCAMAAADsvyBXAAACUlBMVEUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADY2Nijo6MAAACsrKytra0EBARCQkIsLCzBwcGvr68SEhIjIyN8fHwAAACFhYWKioppaWnU1NSdnZ3d3d1GRkY3NzcJCQktLS0fHx9ZWVmcnJybm5u9vb3Dw8NGRkY0NDQlJSVUVFQxMTE3Nzc+Pj5fX19tbW1zc3M/Pz9xcXGhoaFGRkYCAgJtbW0ODg5lZWUYGBhBQUFVVVVGRkZNTU1YWFhHR0cfHx8wMDAnJycVFRUODg4pKSk3NzcQEBA2NjZcXFw1NTV2dnZBQUFGRkZSUlJgYGB3d3eJiYkAAAB0dHQ9PT2srKwiIiJycnKXl5cgICA6Ojp7e3tbW1smJiYYGBhkZGQJCQkQEBBra2tpaWl2dnZkZGRWVlZNTU0bGxtXV1clJSVCQkJkZGR0dHQFBQUxMTFZWVkUFBRkZGRnZ2dvb2/FxcWWlpZTU1NgYGAyMjKUlJSJiYlISEhLS0uSkpKdnZ1QUFBHR0dSUlJCQkJ3d3cGBgaWlpa0tLQkJCRXV1f9/f3////8/Pz6+vrk5OTBwcH7+/sAAACDg4P39/fs7Oz19fXw8PCfn5+ZmZmAgIBHR0fq6urc3NzY2NjExMSLi4uIiIheXl5UVFTo6Ojf39/IyMhMTEzV1dW5ublvb2+9vb2vr6+oqKg1NTXLy8u1tbWysrJ9fX1ra2suLi7R0dHOzs6jo6OUlJR4eHhYWFhQUFBCQkI+Pj4hISGPj485OTknJydbwVDUAAAAj3RSTlMAdxHd7iKZzESqVTOIuwYEZi0lt+wlFgv0021pYDgtIh8PxMS3tbCDSSwnHPfy8uzk2qCKhnNiU0IzMhb+/f329evp29vb2czGxr66t7SqpoqJf3prZFZLQD04MCEY9O/t7Ozr6ubj2c3JxbSupJ+dmZiVlZSTh3lta2hZWEhGQDAj9/Luy8m9qKOjmJVwbN0WgJwAAAa1SURBVFjD3ZcFUxtRFIXvWjYutMVaaKm7u7u7u7u7u3tuhIQkhAQoBKdQd/9ffWGT7m662aalkuk3DJNJMjsf597zloX/CUuW7WSPJUPmz190fOlaC/wqNPwZMpZ0z+lXWeINPql/9ck7aXaPXvBLaPk/YWjOG9z/fUlNtR1jVD1+2/+AzfwLfgwHv59OC8Y/e1SIBHsMRHxROXah+Wenq2H0f2DGy7ZFXkblHHYJiPjkuakT/AQcb9CYwKoBOa2OdMn4ILFTAF2RnIyf8CPp6RkuccYaVgut4uT4cuKnhAPt3pkpZ0jxRqCBYShaBxKIcuvKPiJb8FMEsXhvAaSGlaU5HigTULIZ66jWlb3X9gfJ/chW4uvDKR58NGsw6cHAJvSYMRhMmlaUfV8l8VMBsWQ5qGJkYoY6Rgca2iRZOQNFBCnWyGhBTuplP/OlDAslNtIXAlg2Vb0oeg8VS4XltDxwBrE1LFHW8lrQWUGOUPYUMHf7JMmvqMofe1XtkhhWDAVVeD42aUbP62W10dFMi1pigtKyq9PhvV9i8uRjEwrHy/P3PvHMDg+wqdfXY2iJy0B5KHlOFMfwQn2kZdJQIJZdHXOXCrRLovIIeWLZWM9LURCfHlUvCRv1MhELvTS9qCBDMXqOlx6EtJE1srRYdnVW96uWbmB5qFEQLHrzLIyiYdlA9S3UMdFNlO8ZR6w0Hg0YdcDREj+e4QwMiGVX59gzaYMLC9Eu6DrJj1NSnYYhP6iJFnhTYg90QPHExCQ/FClGiC5a9h/SZ/oTtEuJCzqIrUPyduP0juozNulYTv4WyYjWsnRC1EYA3sryJoomZf8x5z66UKZH1L4JSj9whjqAGhqPtB400WAMwJNYZaUxkJRpoFiTlSLF/x7L2ryV+WNAwuIIyhOsr8bY1j10ypJt2gOqWPWyQLVAmqqxgjwmLroJHM0YFP/DuXCwy8CJ4wZMnbfvvg1iDPZiwk3DHROs+VAnyzYQGSn+oRln27cwXFYd+SQplktoKcWRQI1GErdRKb0e/RselwWcfpe76d36ExZhBae9SiboqyWvRBzoHRw7mEbuvzagtvJNyQNvQ2hyl2GZiqXhwcqw8vmCSQMUw9CUXtjMBE6FSvEbL0JCKfP7l8oF/ZXlGFMdVyz/qG6QoJL7PFgaKMIWCsMvgwOHK5bGYNXoE4IiXjSrpxkOFOrbeyrxE4PyRc4AIa9vEcpwhR5h7DsNlSgn0rPlQpvdKEC+E6V0XK5SaTSUVE+475JEddT38y3onXd+5cH4sjkc0V9YPmeVdsWqIR/c5TWPampqyt1RGt2vP8cjxeIPZf4qV1W4qioc9vl8YXvF3oz8/E4niLd4oUKHA12bemb07lgAMjiW1cojpYmgDrQeHSTS8fTCm9221CNWV0en4hTuFH3bdO/evU2bubPndo3SJsrcbrM+++KC1c9ndtu9a+eUKVMmtY0yacO2nOk5OVvit0anA+3OImd0bzvfuXs68Zjk9Am1IenxLAd6UMACWbvrEIuLEV0u4WkosLETWCwWSOBi32+CVZ/PA5izxowZc6ldC5d6jVozqmOHN2gnV0GH3eUj8/W6EJt6QIHsSsq1oRhap4EkmK+7BMH612gvKirEql1ZoMCaCf64oKN2KSiwqhYRK0iBsJ5c7rHnIWLjUEgBhjUAZ0wqOIMk+DSIWB4JIAbrsVT53B09oQjjTXpwBBTInxxAfPuACD6sdBBBL+KTHpACRg2oMe8V+pqJYFNzGLGhBBu7/VDwlaKguctDDDSXRAUjTnx8bGcAK5ZDChgYUGPpM9zY/SpJMfslYm0Eiw8pC/a1fxN82FlxsYY1oLvtFUR88yVAnlYOu8NtbZAKRhpUyOzcvLDTDifuOBrEss6d3Unu/aMuO78J1m2+CAp0HFQTPHWrFJ0TBzb6J/dZveH5UPgdZPaywPxHZXPyt6I3t2e//VmgxMh34oirx64EJZZ96WfLrcC6G+cGNAwGsPUyw+9i5KZ3uXDo3Yx1kJnkoiNqxQQDzcopm4ctg96Dmp4OhdwDGfB7GbWiANataAdJOSsK2vHtYkhK3qx5mfAPGB4qFAW9iyA5ZjP8C+6FJAk+WABpx/D3fongLAukGyOaXaJgxbRMSDdWr38hSXD7Wkg3RmfXSQSnrYN0Y012jURwZvrtYFaX16Lg0zRsMSx6ixjzw9rFkH70/BhvCbo+joD0wzw7/lCE9YP6QBrSMzuIiA47+kLDIC3pkP3skR/95R/mFEB6knc7Z6J364zjaTlggSzbBdvfOwK/Ao7rn6nz1xGPAAAAAElFTkSuQmCC\" alt=\"Lee was here\">"
});
