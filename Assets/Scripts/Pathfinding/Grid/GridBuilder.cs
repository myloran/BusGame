using System;
using System.Linq;
using Plugins.Ext;
using TMPro;
using UnityEngine;

namespace Pathfinding {
  public class GridBuilder : MonoBehaviour {
    public NodeBuildingService NodeBuildingService;
    public NGrid Grid;
    public Camera Camera;
    public TMP_Dropdown DNode;
    public string[] NodeLayers;

    ENode selectedNode;

    public void Init() {
      var names = Enum.GetNames(typeof(ENode));
      var options = names.Select(n => new TMP_Dropdown.OptionData(n));
      DNode.options.Clear();
      DNode.options.AddRange(options);
      if (names.Any()) DNode.captionText.text = names[0];

      DNode.onValueChanged.AddListener(SetSelectedNode);
    }

    void SetSelectedNode(int nodeIndex) {
      selectedNode = (ENode) nodeIndex;
    }

    public void Update() {
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