using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gmap.Gameplay;
using Gmap.CosmicMusicUtensil;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Frictionless;
using NUnit.Framework;

public class GameflowTests 
{
    private IEnumerator LoadMenu(System.Action<Menu> OnMenuLoaded)
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        Scene menuScene = SceneManager.GetSceneAt(SceneManager.sceneCount-1);
        while (!menuScene.isLoaded)
            yield return new WaitForSecondsRealtime(0.1f);
        var rootObjects  = menuScene.GetRootGameObjects();
        Menu m = rootObjects.SelectMany(g => g.GetComponentsInChildren<Gmap.Gameplay.Menu>()).FirstOrDefault();
        Assert.IsNotNull(m, "Menu can't be null.");
        OnMenuLoaded(m);
    }

    private UnityEngine.UI.Button GetButton(Menu m, string button)
    {
        var buttons = m.GetComponentsInChildren<UnityEngine.UI.Button>();
        var btn = buttons.FirstOrDefault(b => b.gameObject.name.ToLower().Replace(" ", "").Contains(button));
        Assert.IsNotNull(btn, $"Button {button} wasn't found.");
        return btn;
    }

    private T Get<T>(Menu m, string name) where T : Behaviour
    {
        var objects = m.GetComponentsInChildren<T>();
        var component = objects.FirstOrDefault(b => b.gameObject.name.ToLower().Replace(" ", "").Contains(name));
        Assert.IsNotNull(component, $"Component {name} wasn't found.");
        return component;
    }

    [UnityTest]
    public IEnumerator StartsAtInsertCoin()
    {
        Menu menu = null;
        yield return LoadMenu((m) => {
            menu = m;
        });
        yield return new WaitForSeconds(1f);
        Assert.IsTrue(menu.transform.Find("Title").gameObject.activeSelf);
        Assert.IsTrue(menu.transform.Find("PressStart").gameObject.activeSelf);
        Assert.AreEqual(GameState.Current, Resources.Load<GameState>("GameStates/MenuPressStart"));
    }

    [UnityTest]
    public IEnumerator PressStartTransitionsToMenu()
    {
        Menu menu = null;
        yield return LoadMenu(m => menu = m);
        var newGameButton = GetButton(menu, "press");
        Assert.NotNull(newGameButton, "Could not find new game button.");
        newGameButton.onClick.Invoke();
        Assert.AreEqual(GameState.Current, Resources.Load<GameState>("GameStates/MenuSelectOption"));
        yield break;
    }

    [UnityTest]
    public IEnumerator NewGameTransitionsToGameOptions()
    {
        Menu menu = null;
        yield return LoadMenu((m) => menu = m);

        GetButton(menu, "press").onClick.Invoke();
        var newGameButton = GetButton(menu, "new");
        Assert.IsNotNull(newGameButton, "Could not find new game button.");
        newGameButton.onClick.Invoke();

        Assert.AreEqual(GameState.Current, Resources.Load<GameState>("GameStates/MenuSelectMelody"));
    }

    [UnityTest]
    public IEnumerator StandardModeTransitionsToGame()
    {
        Menu menu = null;
        yield return LoadMenu((m) => menu = m);

        GetButton(menu, "press").onClick.Invoke();
        GetButton(menu, "new").onClick.Invoke();
        var buttonRandom = GetButton(menu, "random");
        Assert.IsNotNull(buttonRandom, "Could not find standard melody button.");

        buttonRandom.onClick.Invoke();
        yield return new WaitForSeconds(3f);
        Assert.AreEqual(GameState.Current, Resources.Load<GameState>("GameStates/GameplayPlay"));
    }

    [UnityTest]
    public IEnumerator CustomModeTransitionsToCustomMelodySelection()
    {
        Menu menu = null;
        yield return LoadMenu((m) => menu = m);

        GetButton(menu, "press").onClick.Invoke();
        GetButton(menu, "new").onClick.Invoke();
        var buttonRandom = GetButton(menu, "custom");
        Assert.IsNotNull(buttonRandom, "Could not find custom melody button.");
        buttonRandom.onClick.Invoke();
        Assert.AreEqual(GameState.Current, Resources.Load<GameState>("GameStates/MenuCustomMelody"));
    }

    [UnityTest]
    public IEnumerator CustomMelodySelectionTransitionsToGame()
    {
        Menu menu = null;
        yield return LoadMenu((m) => menu = m);

        GetButton(menu, "press").onClick.Invoke();
        GetButton(menu, "new").onClick.Invoke();
        GetButton(menu, "custom").onClick.Invoke();
        Get<TMPro.TMP_InputField>(menu, "custommelody").text = "c4/4;c4/4;c4/4;c4/4";
        Get<UnityEngine.UI.Button>(menu, "ok").onClick.Invoke();
        yield return new WaitForSeconds(3f);
        Assert.AreEqual(GameState.Current, Resources.Load<GameState>("GameStates/GameplayPlay"));

        var playerInstrument = LevelLoader.CurrentLevelConfiguration.GetInstrumentConfigurationByTag("Player");
        Melody m = playerInstrument.MelodyFactory.GenerateMelody();
        Assert.AreEqual(m.Notation, "c4/4;c4/4;c4/4;c4/4");
    }
}
