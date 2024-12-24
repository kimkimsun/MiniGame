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

    // WebSocket 초기화 및 연결
    function initializeWebSocket() {
      socket = new WebSocket("ws://10.47.0.8:8007");

      socket.onopen = async () => {
        console.log("WebSocket 연결 성공 !");

        try {
          const response = await fetch("/ranking", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
          });
          const data = await response.json();

          if (response.ok) {
            const names = data.map((row) => row.name).join(","); // 배열을 문자열로 결합
            const scores = data.map((row) => row.score).join(","); // 배열을 문자열로 결합

            // 두 문자열을 하나로 결합해서 보내기
            const combined = names + "|" + scores; // 이름과 점수를 '|'로 구분해서 결합

            // Unity로 데이터 전송
            if (typeof globalUnityInstance !== 'undefined' && globalUnityInstance) {
              globalUnityInstance.SendMessage(
                "RANKING", // Unity 오브젝트 이름
                "RankSettings", // Unity 메서드 이름
                combined // 결합된 하나의 문자열 전달
              );
            }
          } else {
            alert(data.error);
          }
        } catch (error) {
          console.error("랭킹 조회 중 오류 발생:", error);
        }
      };

      socket.onclose = () => {
        console.log("WebSocket 연결 재 시도중...");
        setTimeout(initializeWebSocket, 1000); // 재연결 시도
      };

      socket.onerror = (error) => {
        console.error("WebSocket 연결 에러", error);
      };

      socket.onmessage = (event) => {
        console.log("WebSocket 서버로 부터 메시지를 받았습니다.", event.data);
      };
    }

    // WebSocket 초기화 호출
    initializeWebSocket();
  },

  SendScore: function (userName, score) {
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