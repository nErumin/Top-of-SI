﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Model;

public class LobbyManager : MonoBehaviour
{
    public Player CurrentPlayer
    {
        get; private set;
    }

    public static LobbyManager Instance
    {
        get; private set;
    }

    private GameStage selectedStage;

    public GameStage SelectedStage
    {
        get
        {
            return selectedStage;
        }
        set
        {
            if (value != null)
            {
                selectedStage = value;
                OnChangeStage(value, isFirstProject(value), isLastProject(value));
            }
        }
    }

    List<GameStage> stages;

    [SerializeField]
    private LobbyUiPresenter lobbyUi;

    public event Action<GameStage, bool, bool> OnChangeStage = delegate { };

    private void Awake()
    {
        if (Instance != null)
        {
            DebugLogger.LogWarning("LobbyManager::Awake => 이미 초기화된 LobbyManager가 메모리에 존재합니다.");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void TempLobby()
    {
        stages = new List<GameStage>{
            new GameStage
            {
                Title = "Test Project",
                ElapsedDayLimit = 5,
                Boss = new ProjectSpec()
                {
                    Ability = new TestProject().Ability,
                    Status = new TestProject().Status
                },
                IconName = "Cpp"
            },
            new GameStage
            {
                Title = "Web Project",
                ElapsedDayLimit = 4,
                Boss = new ProjectSpec()
                {
                    Ability = new TestProject().Ability,
                    Status = new TestProject().Status
                },
                IconName = "CSharp"
            },
            new GameStage
            {
                Title = "iOS Project",
                ElapsedDayLimit = 3,
                Boss = new ProjectSpec()
                {
                    Ability = new TestProject().Ability,
                    Status = new TestProject().Status
                },
                IconName = "Java"
            }
        };



        stages[0].AddObjectives(new List<IStageObjective>
            {
                new ElapsedDayObjective(stages[0]),
                new StringObjective("테스트 목표 1"),
                new StringObjective("테스트 목표 2"),
                new StringObjective("테스트 목표 3"),
            });

        stages[1].AddObjectives(new List<IStageObjective>
        {
            new ElapsedDayObjective(stages[1]),
            new StringObjective("테스트 목표"),
        });
        stages[2].AddObjectives(new List<IStageObjective>
        {
            new ElapsedDayObjective(stages[2]),
            new StringObjective("테스트 목표"),
        });

        selectedStage = stages[0];
    }

    private void Start()
    {
        CurrentPlayer = new Player();
        stages = new List<GameStage>();
        TempLobby();
        //SaveStages();
        //LoadStages();
        OnChangeStage += lobbyUi.UpdateProject;
        SelectedStage = stages[0];

        // TODO: Temporary Load/Save
        //var savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Player.xml";
        
        // Read
        //var rootElement = XElement.Parse(File.ReadAllText(savePath));
        //CurrentPlayer.RecoverStateFromXml(rootElement.ToString());

        // Write
        //File.WriteAllText(savePath, CurrentPlayer.ToXmlElement().ToString());
    }

    public void LoadStages()
    {
        var projectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Stage.xml";
        var rootElement = XElement.Parse(File.ReadAllText(projectPath));

        foreach (var stageElement in rootElement.Elements("Stage"))
        {
            var newStage = new GameStage();
            newStage.RecoverStateFromXml(stageElement.ToString());
            stages.Add(newStage);
        }
    }

    public void SaveStages()
    {
        var projectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Stage.xml";
        var stageRootElement = new XElement("Stages",
            stages.Select(stage => stage.ToXmlElement()));

        File.WriteAllText(projectPath, stageRootElement.ToString());
    }

    public void ChangeToNextProject()
    {
        var index = stages.IndexOf(selectedStage);

        if (index == (stages.Count - 1))
        {
            return;
        }
        else
        {
            SelectedStage = stages[index + 1];
        }
    }

    public void ChangeToPreviousProject()
    {
        var index = stages.IndexOf(selectedStage);

        if (index == 0)
        {
            return;
        }
        else
        {
            SelectedStage = stages[index - 1];
        }
    }

    bool isFirstProject(GameStage stage)
    {
        if (stages.IndexOf(stage) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool isLastProject(GameStage stage)
    {
        if (stages.IndexOf(stage) == (stages.Count - 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
