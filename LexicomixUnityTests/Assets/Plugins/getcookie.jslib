mergeInto(LibraryManager.library, {

  HelloString: function (str) {
    window.alert(Pointer_stringify(str));
  },

  getCookie: function (cname) {
       var ret="";
       var name = Pointer_stringify(cname) + "=";
       console.log('name='+name);  
       console.log('cookie='+document.cookie);     
       var decodedCookie = decodeURIComponent(document.cookie);
       console.log('decoded cookie='+decodedCookie);
       var ca = decodedCookie.split(';');
       for(var i = 0; i <ca.length; i++) {
           var c = ca[i];
           while (c.charAt(0) == ' ') {
               c = c.substring(1);
           }
           if (c.indexOf(name) == 0) {
               ret=c.substring(name.length, c.length);
               break;
           }
       }
       var buffer = _malloc(lengthBytesUTF8(ret) + 1);
       stringToUTF8(ret, buffer, 1 + lengthBytesUTF8(ret));
       return buffer;
    },

});