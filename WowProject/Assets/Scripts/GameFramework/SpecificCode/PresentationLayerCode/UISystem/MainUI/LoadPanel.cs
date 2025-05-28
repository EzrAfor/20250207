using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//*****************************************
//创建人： Trigger 
//功能说明：加载面板
//***************************************** 
public class LoadPanel : BasePanel
{
    private Slider processViewSli;
    public override void OnInit()
    {
        processViewSli = transform.Find("Slider").GetComponent<Slider>();
        base.OnInit();
    }

    public override void OnShow(params object[] objs)
    {
        base.OnShow(objs);
        StartCoroutine(startLoading(1));
    }

    private IEnumerator startLoading(int sceneIndex)
    {
        int displayProgress = 0;
        int toProgress = 0;
        AsyncOperation ao= SceneManager.LoadSceneAsync(sceneIndex);
        ao.allowSceneActivation = false;
        while (ao.progress<0.9f)
        {
            toProgress = (int)ao.progress * 100;
            while (displayProgress<toProgress)
            {
                displayProgress++;
                SetLoadingPercentValue(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }
        toProgress = 100;
        while (displayProgress<toProgress)
        {
            displayProgress++;
            SetLoadingPercentValue(displayProgress);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        ao.allowSceneActivation = true;
        OnClose();
    }

    private void SetLoadingPercentValue(int value)
    {
        processViewSli.value = value / 100;
    }
}
