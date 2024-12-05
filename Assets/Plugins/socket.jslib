mergeInto(LibraryManager.library, {
  Hello: function () {
    window.alert("Hello, world!");
  },

  HelloString: function (str) {
    window.alert(UTF8ToString(str));
  },

  PrintFloatArray: function (array, size) {
    for (var i = 0; i < size; i++) console.log(HEAPF32[(array >> 2) + i]);
  },

  AddNumbers: function (x, y) {
    return x + y;
  },

  StringReturnValueFunction: function () {
    var returnStr = "bla";
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  WebSocketSetting: function () {
    let socket;
    function initializeWebSocket() {
      socket = new WebSocket("ws://localhost:8007");
      socket.onopen = () => {
        console.log("WebSocket 연결 성공 !");
      };
      socket.onclose = () => {
        console.error("WebSocket 닫힘");
        setTimeout(initializeWebSocket, 1000); // 재연결 시도
      };
      socket.onerror = (error) => {
        console.error("WebSocket 오류", error);
      };
      socket.onmessage = (event) => {
        console.log("WebSocket 메시지", event.data);
      };
    }
    initializeWebSocket();
  },

  SendScore: function (score) {
    if (socket && socket.readyState === WebSocket.OPEN) {
      socket.send(score);
    } else {
      console.error("WebSocket 연결이 열려 있지 않습니다.");
    }
  },

  BindWebGLTexture: function (texture) {
    GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
  },
});