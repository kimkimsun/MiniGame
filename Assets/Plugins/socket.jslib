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
        console.log("WebSocket ���� ���� !");
      };
      socket.onclose = () => {
        console.error("WebSocket ����");
        setTimeout(initializeWebSocket, 1000); // �翬�� �õ�
      };
      socket.onerror = (error) => {
        console.error("WebSocket ����", error);
      };
      socket.onmessage = (event) => {
        console.log("WebSocket �޽���", event.data);
      };
    }
    initializeWebSocket();
  },

  SendScore: function (score) {
    if (socket && socket.readyState === WebSocket.OPEN) {
      socket.send(score);
    } else {
      console.error("WebSocket ������ ���� ���� �ʽ��ϴ�.");
    }
  },

  BindWebGLTexture: function (texture) {
    GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
  },
});