using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch4pkeditorM
{
    public static class Constant
    {
        public enum Errors
        {
            CORE_WRONG_PROCESS,
            CORE_UNEXPECTED_ERROR_WHEN_OPEN,
            CORE_DATA_NOT_READY,
            CORE_PROCESS_NOT_AVAILABLE
        };

        public static Dictionary<Errors, string> ErrorsDescription = new Dictionary<Errors, string>()
        {
            { Errors.CORE_WRONG_PROCESS, "錯誤的Process。" },
            { Errors.CORE_UNEXPECTED_ERROR_WHEN_OPEN, "開啟Process過程中發生錯誤。" },
            { Errors.CORE_DATA_NOT_READY, "尚未讀取遊戲資料!" },
            { Errors.CORE_PROCESS_NOT_AVAILABLE, "遊戲已關閉或無法讀取。" }
        };

        public enum Messages
        {
            FORM_LOADING_DATA,
            FORM_LOAD_FINISH,
            FORM_PLEASE_OPEN_GAME_FIRST,
            FORM_SAVED_RETURN_GAME,
            FORM_SAVE_FAILED
        };

        public static Dictionary<Messages, string> MessagesDescription = new Dictionary<Messages, string>() {
            { Messages.FORM_LOADING_DATA, "讀取資料中，請稍後。" },
            { Messages.FORM_LOAD_FINISH, "讀取完畢。" },
            { Messages.FORM_PLEASE_OPEN_GAME_FIRST, "請先開啟遊戲。若已開啟遊戲仍偵測不到，請至論壇回報，並依照網站敘述提供資訊，謝謝。" },
            { Messages.FORM_SAVED_RETURN_GAME, "已儲存，請返回遊戲刷新查看。" },
            { Messages.FORM_SAVE_FAILED, "寫入錯誤。" }
        };
    }
}
