using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;

public class GUICarousel
{
    // Delegates
    public delegate void OnDrawElementCallbackDelegate(Rect rect, SerializedProperty element, bool isSelected);
    public delegate void OnDrawToolbarCallbackDelegate(Rect rect);

    private const int NEIGHBOUR_POSITION_LEFT = -1;
    private const int NEIGHBOUR_POSITION_RIGHT = 1;
    private readonly float TOOLBAR_HEIGHT;

    private const float CLIPPING_OFFSET = 25f;

    private SerializedObject m_Target;
    private SerializedProperty m_Array;

    private UnityAction m_Repaint;
    private RectOffset m_ElementSpacing;
    private AnimFloat m_ScrollPosition;

    private int m_SelectedIndex = 0;
    private float m_ElementWidth;
    private float m_ElementHorizontalSize;
    private int m_ElementCount;

    // User Actions
    private OnDrawElementCallbackDelegate m_OnDrawElementCallback;
    private OnDrawToolbarCallbackDelegate m_OnDrawToolbarCallback;


    /// <summary>
    /// Invoked for every element that is going to be drawn by the
    /// GUICarousel. Only one listener can be subscribed at a time. 
    /// </summary>
    public event OnDrawElementCallbackDelegate onDrawElementCallback
    {
        add { m_OnDrawElementCallback = value; }
        remove { m_OnDrawElementCallback -= value; }
    }

    /// <summary>
    /// Invoked whenever we have a GUI request to draw the toolbar. If 
    /// none is created a default one will be done for you. Only one listner
    /// can be subscribed at a time.
    /// </summary>
    public event OnDrawToolbarCallbackDelegate onDrawToolbarCallback
    {
        add { m_OnDrawToolbarCallback = value; }
        remove { m_OnDrawToolbarCallback -= value; }
    }

    /// <summary>
    /// Gets or sets the speed at which the elements scroll.
    /// </summary>
    public float speed
    {
        get
        {
            return m_ScrollPosition.speed;
        }
        set
        {
            m_ScrollPosition.speed = value;
        }
    }

    /// <summary>
    /// Gets the array element at the current index or null if 
    /// we have no elements. 
    /// </summary>
    public SerializedProperty selectedElement
    {
        get
        {
            if (m_ElementCount > 0)
            {
                return m_Array.GetArrayElementAtIndex(m_SelectedIndex);
            }
            return null;
        }
    }

    public GUICarousel(SerializedProperty serializedArray, UnityAction repaintCallback)
    {
        m_Array = serializedArray;
        m_Target = m_Array.serializedObject;
        m_ElementSpacing = new RectOffset(3, 3, 6, 6);
        m_ScrollPosition = new AnimFloat(0.0f);
        m_ScrollPosition.valueChanged.AddListener(repaintCallback);
        m_ScrollPosition.speed = 0.55f;
        m_Repaint = repaintCallback;
        TOOLBAR_HEIGHT = EditorGUIUtility.singleLineHeight;
    }


    public void DoGUILayout()
    {
        // Get our working rect
        Rect windowRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight * 8);
        // Set our width
        m_ElementWidth = windowRect.height - EditorGUIUtility.singleLineHeight;
        // Call the GUI function
        DoGUI(windowRect);
    }

    public void DoGUI(Rect windowRect)
    {
        // Save our full element width
        m_ElementHorizontalSize = m_ElementSpacing.horizontal + m_ElementWidth;
        m_ElementCount = m_Array.arraySize;

        GUI.Box(windowRect, GUIContent.none);

        Rect toobarRect = windowRect;
        toobarRect.height = TOOLBAR_HEIGHT;

        if (m_OnDrawToolbarCallback == null)
        {
            m_OnDrawToolbarCallback = DefaultDrawToolbar;
        }
        m_OnDrawToolbarCallback(toobarRect);

        // Start our scroll offset
        float elementsPositionOffset = 0.0f;
        // Force it to the center
        elementsPositionOffset += (windowRect.width / 2f) - (0.5f * m_ElementHorizontalSize);
        // Offset our position by our scroll position
        elementsPositionOffset -= m_ScrollPosition.value;

        DrawSelectionNeighbours(windowRect, elementsPositionOffset, false, NEIGHBOUR_POSITION_LEFT);
        DrawSelectionNeighbours(windowRect, elementsPositionOffset, true, NEIGHBOUR_POSITION_RIGHT);

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.RightArrow)
        {
            Next();
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.LeftArrow)
        {
            Previous();
        }
    }

    private void Next()
    {
        m_ScrollPosition.target += m_ElementHorizontalSize;
        m_SelectedIndex++;
        if (m_SelectedIndex >= m_ElementCount)
        {
            m_SelectedIndex = 0;
            m_ScrollPosition.value -= m_ElementCount * m_ElementHorizontalSize;
            m_ScrollPosition.target = 0f;
        }
    }

    private void Previous()
    {
        m_ScrollPosition.target -= m_ElementHorizontalSize;
        m_SelectedIndex--;
        if (m_SelectedIndex < 0)
        {
            m_SelectedIndex = m_ElementCount - 1;
            m_ScrollPosition.value += m_ElementCount * m_ElementHorizontalSize;
            m_ScrollPosition.target = m_ElementHorizontalSize * m_SelectedIndex;
        }
    }

    private void DrawSelectionNeighbours(Rect windowRect, float scrollPositionOffset, bool drawSelection, int direction)
    {
        // Get our rect
        Rect elementRect = windowRect;
        // Subtract our height
        elementRect.height = m_ElementWidth - m_ElementSpacing.vertical;
        // Move the element down.
        elementRect.y += m_ElementSpacing.top + TOOLBAR_HEIGHT;
        // Mod index
        int modIndex = m_SelectedIndex;
        // Loop over all elements.
        int loopCount = 0; // 

        if (m_ElementCount < 3)
        {
            loopCount = 1;
        }
        else
        {
            loopCount = (m_ElementCount - 1) / 2;
        }

        // Set our starting value
        for (int i = drawSelection ? 0 : direction; Mathf.Abs(i) < loopCount + 1; i += direction)
        {
            // Get our base selection
            int elementIndex = m_SelectedIndex + i;
            int positionSclaer = m_SelectedIndex + i;

            if (elementIndex >= m_ElementCount)
            {
                elementIndex = elementIndex % m_ElementCount;
            }
            else if (elementIndex < 0)
            {
                elementIndex = m_ElementCount + elementIndex;
            }

            // Start from the left
            elementRect.x = scrollPositionOffset;
            // Move to the middle
            elementRect.x += (m_ElementHorizontalSize * positionSclaer);
            // Set width
            elementRect.width = m_ElementWidth;
            // Get our current index;
            SerializedProperty element = m_Array.GetArrayElementAtIndex(elementIndex);
            // Check for selection input.
            if (Event.current.type == EventType.MouseDown && elementRect.Contains(Event.current.mousePosition))
            {
                // The use clicked a element and we just keep iterating until we get there.
                while (m_SelectedIndex != elementIndex)
                {
                    if (direction > 0) { Next(); }
                    else { Previous(); }

                }
                Event.current.Use();
            }

            // Are we too far off to the left or right? If so don't draw the element or make any callbacks.
            //if ((elementRect.xMax > windowRect.x + CLIPPING_OFFSET) && (elementRect.xMin < windowRect.x + windowRect.width - CLIPPING_OFFSET))
            {
                // Invoke our draw function
                if (m_OnDrawElementCallback == null)
                {
                    m_OnDrawElementCallback += DefaultDrawElement;
                }
                m_OnDrawElementCallback(elementRect, element, m_SelectedIndex == elementIndex);
            }
        }
    }


    private void DefaultDrawToolbar(Rect rect)
    {
        // Draw our toolbar background
        GUI.Box(rect, GUIContent.none, EditorStyles.toolbar);
    }

    private void DefaultDrawElement(Rect rect, SerializedProperty property, bool isSelected)
    {
        if (isSelected)
        {
            GUI.color = Color.gray;
        }
        GUI.Box(rect, new GUIContent(property.displayName));
        GUI.color = Color.white;
    }
}