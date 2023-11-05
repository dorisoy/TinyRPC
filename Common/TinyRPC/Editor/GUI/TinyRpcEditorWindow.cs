﻿using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
// todo : 整合编辑器设置和运行时设置，编辑器下做代码生成，运行时做 Assembly 过滤和 Log 过滤
// todo:  支持将生成的代码放置在 3 个不同文件夹，分别是 Assets 内、Project 同级目录，Packages 文件夹
// Assets 内： 支持存到 Assets 下的任意路径             ， 路径是 ：TinyRPC/Generated
// Project 同级：方便代码公用，但是需要我生成 package.json ，顺便 version 自增，同时还要自动加到 manifest.json 中 , 路径是：../../TinyRPC Generated
// Packages 文件夹：方便代码公用，但是需要我生成 package.json ，顺便 version 自增 ，路径是：Packages/TinyRPC Generated
// 以上需要控制编辑器编译时机，这个 API 需要慎重，生成代码前关闭自动编译，代码生成完成或者代码自动生成过程中抛异常一定要重新开启编译功能
// 计划是使用 Tab 页签切换，页签分别是：Editor 、Runtime

//支持多个 proto 文件，逻辑是：
// 为每个proto 文件内的消息创建一个文件夹，文件夹名字为 proto 文件名字，为所有消息生成以消息名字命名的 cs 单文件
// 如果位于不同文件夹的 proto 同名，则生成的 cs 文件会放在同一个文件夹下，重复的消息仅作告警处理
// 由于消息的量级可能会越来越大， proto 匹配的文件夹中还会生成 Normal + RPC 文件夹
// Normal 文件夹中存放的是普通消息
// RPC 文件夹中存放的是 RPC 消息,并且 RPC 消息是 Request + Response 生成在同一个 .cs 文件中，方便查看

// Editor Tab 需要有一个输入框+按钮组成的消息查询功能，避免用户遗忘消息名字，导致无法找到对应的消息，抽象查询，高亮展示在 Tab 查询功能下方，且具备下拉框功能，ping 消息所在的文件 
// 由于没有消息 ID这一说法，所以，可能不需要查询功能，或者简单的查询
// Runtime Tab, 编辑器下 Assembly 最好使用 AssemblyDefinitionFile ，方便 Ping, 实际上存的依旧是 Assembly.Name,判断依旧是  StartWith
// Runtime Tab ,编辑器下 Log Filter 最好使用 高级下拉窗口，方便用户选择，实际上存的依旧是 Type.Name,判断依旧是  Contain 就不输出收到网络消息的 log,比如 ping

namespace zFramework.TinyRPC.Editor
{
    public class TinyRpcEditorWindow : EditorWindow
    {
        private RuntimeSettingsLayout runtimeSettingsLayout;
        private EditorSettingsLayout editorSettingsLayout;

        int selected = 0;
        static GUIContent[] toolbarContents;

        [MenuItem("Tools/.proto 转 .cs 实体类")]
        public static void ShowWindow() => GetWindow(typeof(TinyRpcEditorWindow));
        public void OnEnable()
        {
            // title with version
            var package = PackageInfo.FindForAssembly(typeof(TinyRpcEditorWindow).Assembly);
            var version = package.version;
            titleContent = new GUIContent($"TinyRPC (ver {version})");
            minSize = new Vector2(360, 220);

            //init layout instance
            runtimeSettingsLayout = new RuntimeSettingsLayout(this);
            editorSettingsLayout = new EditorSettingsLayout(this);

            // init Editor Settings
            toolbarContents = new GUIContent[] { BT_LT, BT_RT };
        }

        private void OnGUI()
        {
            //draw tab Editor and Runtime
            GUILayout.Space(10);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                var idx = EditorPrefs.GetInt(key, 0);
                selected = GUILayout.Toolbar(idx, toolbarContents, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.2f));

                if (selected != idx)
                {
                    idx = selected;
                    EditorPrefs.SetInt(key, idx);
                }
                GUILayout.FlexibleSpace();
            }
            if (selected == 0)
            {
                // Draw Editor Settings 
                editorSettingsLayout.Draw();
            }
            else
            {
                //Draw Runtime Settings
                runtimeSettingsLayout.Draw();
            }
        }

        #region GUIContents for tabs
        static GUIContent BT_LT = new GUIContent("Editor", "编辑器下使用的配置");
        static GUIContent BT_RT = new GUIContent("Runtime", "运行时使用的配置");
        const string key = "TinyRPC Tab Index";
        #endregion
    }
}