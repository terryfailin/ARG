﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcAp.Lang {
    using System;
    
    
    /// <summary>
    ///   用於查詢當地語系化字串等的強類型資源類別。
    /// </summary>
    // 這個類別是自動產生的，是利用 StronglyTypedResourceBuilder
    // 類別透過 ResGen 或 Visual Studio 這類工具。
    // 若要加入或移除成員，請編輯您的 .ResX 檔，然後重新執行 ResGen
    // (利用 /str 選項)，或重建您的 VS 專案。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Language {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Language() {
        }
        
        /// <summary>
        ///   傳回這個類別使用的快取的 ResourceManager 執行個體。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MvcAp.Lang.Language", typeof(Language).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   覆寫目前執行緒的 CurrentUICulture 屬性，對象是所有
        ///   使用這個強類型資源類別的資源查閱。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查詢類似 帳號 的當地語系化字串。
        /// </summary>
        public static string Account {
            get {
                return ResourceManager.GetString("Account", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 Back 的當地語系化字串。
        /// </summary>
        public static string Back {
            get {
                return ResourceManager.GetString("Back", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 {0} is empty. 的當地語系化字串。
        /// </summary>
        public static string ErrorMessage_IsEmpty {
            get {
                return ResourceManager.GetString("ErrorMessage_IsEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 您所提供的帳號或密碼不正確 的當地語系化字串。
        /// </summary>
        public static string ErrorMessage_Login {
            get {
                return ResourceManager.GetString("ErrorMessage_Login", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 Invalid Password 的當地語系化字串。
        /// </summary>
        public static string ErrorMessage_Password {
            get {
                return ResourceManager.GetString("ErrorMessage_Password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 驗證碼錯誤 的當地語系化字串。
        /// </summary>
        public static string ErrorMessage_ValidationCode {
            get {
                return ResourceManager.GetString("ErrorMessage_ValidationCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 請輸入驗證碼 的當地語系化字串。
        /// </summary>
        public static string ErrorMessage_ValidationCodeEmpty {
            get {
                return ResourceManager.GetString("ErrorMessage_ValidationCodeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 男 的當地語系化字串。
        /// </summary>
        public static string IsMan {
            get {
                return ResourceManager.GetString("IsMan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 This field is required. 的當地語系化字串。
        /// </summary>
        public static string IsNeedRequired {
            get {
                return ResourceManager.GetString("IsNeedRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 女 的當地語系化字串。
        /// </summary>
        public static string IsWoman {
            get {
                return ResourceManager.GetString("IsWoman", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 No matching items found 的當地語系化字串。
        /// </summary>
        public static string NoData {
            get {
                return ResourceManager.GetString("NoData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 密碼 的當地語系化字串。
        /// </summary>
        public static string Password {
            get {
                return ResourceManager.GetString("Password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 Please enter the same value again. 的當地語系化字串。
        /// </summary>
        public static string PasswordNotEq {
            get {
                return ResourceManager.GetString("PasswordNotEq", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 Save 的當地語系化字串。
        /// </summary>
        public static string Save {
            get {
                return ResourceManager.GetString("Save", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 查詢 的當地語系化字串。
        /// </summary>
        public static string SearchFor {
            get {
                return ResourceManager.GetString("SearchFor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 Submit 的當地語系化字串。
        /// </summary>
        public static string Submit {
            get {
                return ResourceManager.GetString("Submit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 驗證碼 的當地語系化字串。
        /// </summary>
        public static string ValidationCode {
            get {
                return ResourceManager.GetString("ValidationCode", resourceCulture);
            }
        }
    }
}
