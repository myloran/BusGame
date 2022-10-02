using System;
using System.Linq;
using Pathfinding;
using Plugins.Ext;
using TMPro;
using UnityEngine;

namespace DefaultNamespace.Pathfinding.States {
  public class GridBuilderState : BaseState {
    public NodeBuildingService NodeBuildingService;
    public Canvas Canvas;
    public NGrid Grid;
    public Camera Camera;
    public TMP_Dropdown DNode;
    public string[] NodeLayers;

    ENode selectedNode;

    public override void Init() {
      var names = Enum.GetNames(typeof(ENode));
      var options = names.Select(n => new TMP_Dropdown.OptionData(n));
      DNode.options.Clear();
      DNode.options.AddRange(options);
      if (names.Any()) DNode.captionText.text = names[0];
    }

    public override void OnEnter() {
      Canvas.enabled = true;
      DNode.onValueChanged.AddListener(SetSelectedNode);
    }

    public override void OnExit() {
      Canvas.enabled = false;
      DNode.onValueChanged.RemoveListener(SetSelectedNode);
    }

    void SetSelectedNode(int nodeIndex) {
      selectedNode = (ENode) nodeIndex;
    }
    
    public override void OnUpdate() {
      if (Input.GetMouseButtonDown(0)) {
        int mask = LayerMask.GetMask(NodeLayers);
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        bool isHit = Physics.Raycast(ray, out var hit, 100 /*, mask*/);

        if (isHit && !UIExt.IsPointerOverUIElement()) {
          NodeView view = hit.transform.GetComponent<NodeView>();

          if (view != null) {
            NodeView newNode = NodeBuildingService.Build(selectedNode,
              view.Model.X, view.Model.Y, view.Obj.transform.parent);
            Grid.Nodes[view.Model.X, view.Model.Y] = newNode;
            Destroy(view.Obj);
          }
        }
      }
    }
  }
}