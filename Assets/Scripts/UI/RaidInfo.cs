using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TowerDefense
{
    public class RaidInfo : Singleton<RaidInfo>
    {
        Transform _transform;

        Wizard wizard;

        [SerializeField] float ShowAnimationTime = 0.1f;
        [SerializeField] LeanTweenType ShowAnimationTween = LeanTweenType.notUsed;

        int soldiersCount = 0;
        [SerializeField] TextMeshProUGUI SoldiersCountText;

        [SerializeField] TextMeshProUGUI Stats1Text;
        [SerializeField] TextMeshProUGUI Stats2Text;
        [SerializeField] TextMeshProUGUI Stats3Text;

        [SerializeField] GameObject TowerCloudTemplate;
        [SerializeField] LineRenderer ArrowLineTemplate;

        Dictionary<Building, RaidInfoTowerCloud> clouds = new Dictionary<Building, RaidInfoTowerCloud>();
        List<GameObject> arrowLinesList = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();

            _transform = GetComponent<Transform>();
        }

        void Start()
        {
            Hide();
            Wizard.ClickedEvent += Show;
        }

        void Show(Wizard wizard)
        {
            if (InputMouse.selected == wizard)
                return;

            if (InputMouse.selected != null)
                InputMouse.ClearSelection();

            InputMouse.selected = wizard;
            this.wizard = wizard;

            clouds.Clear();
            SoldiersCountText.text = soldiersCount.ToString();

            ArrowLineTemplate.gameObject.SetActive(false);
            TowerCloudTemplate.gameObject.SetActive(false);

            LeanTween.scale(gameObject, Vector3.zero, 0f);
            gameObject.SetActive(true);
            StartShowAnimation();
        }

        void StartShowAnimation()
        {
            _transform.position = wizard.transform.position; // TODO: use attachment point
            LeanTween.scale(gameObject, Vector3.one, ShowAnimationTime)
                .setEase(ShowAnimationTween)
                .setOnComplete(ShowAnimationCompleted);
        }

        void ShowAnimationCompleted()
        {
            foreach (var b in Building.Instances)
            {
                LineRenderer line = Instantiate(ArrowLineTemplate.gameObject, _transform).GetComponent<LineRenderer>();
                line.SetPosition(0, b.transform.position + _transform.position * 0.1f);
                line.SetPosition(1, _transform.position + b.transform.position * 0.1f);
                line.gameObject.SetActive(true);
                arrowLinesList.Add(line.gameObject);

                RaidInfoTowerCloud cloud = Instantiate(TowerCloudTemplate.gameObject, _transform).GetComponent<RaidInfoTowerCloud>();
                cloud.transform.position = b.transform.position + new Vector3(-0.85f, 0.8f, 0); // todo: don't use camera.main
                cloud.gameObject.SetActive(true);
                clouds.Add(b, cloud);

                cloud.Building = b;
                cloud.UpdateText();
                cloud.MoreSoldiersButtonClickedEvent += MoreSoldiersButtonClicked;
                cloud.LessSoldiersButtonClickedEvent += LessSoldiersButtonClicked;
            }
        }

        public void Hide()
        {
            foreach (var b in clouds.Keys)
            {
                clouds[b].MoreSoldiersButtonClickedEvent -= MoreSoldiersButtonClicked;
                clouds[b].LessSoldiersButtonClickedEvent -= LessSoldiersButtonClicked;
                Destroy(clouds[b].gameObject);
            }
            clouds.Clear();

            foreach (var arrowLine in arrowLinesList)
            {
                Destroy(arrowLine);
            }
            arrowLinesList.Clear();

            gameObject.SetActive(false);
        }

        void OnDisable()
        {
            foreach (var b in clouds.Keys)
            {
                clouds[b].MoreSoldiersButtonClickedEvent -= MoreSoldiersButtonClicked;
                clouds[b].LessSoldiersButtonClickedEvent -= LessSoldiersButtonClicked;
            }
        }

        public void MoreSoldiersButtonClicked(RaidInfoTowerCloud cloud)
        {
            cloud.DesiredSoldiersCount += 1;
            cloud.UpdateText();

            soldiersCount += 1;
            SoldiersCountText.text = soldiersCount.ToString();
        }

        public void LessSoldiersButtonClicked(RaidInfoTowerCloud cloud)
        {
            cloud.DesiredSoldiersCount -= 1;
            cloud.UpdateText();

            soldiersCount -= 1;
            SoldiersCountText.text = soldiersCount.ToString();
        }

        public void GoButtonClicked()
        {
            foreach (var b in clouds.Keys)
            {
                if (clouds[b].DesiredSoldiersCount > 0)
                {
                    for (int i = 0; i < clouds[b].DesiredSoldiersCount; i++)
                    {
                        b.RemoveSoldier().AttackWizard(wizard);
                    }
                }
            }

            Hide();
        }
    }
}