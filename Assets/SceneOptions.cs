using UnityEngine;
using System.Collections;

public class SceneOptions {

	#region Fields

	private GenerationType m_oGenerationType;
	private int m_iAdCount;

	//======================================================================

	private GenerationType m_oResetGenType = GenerationType.CompleteSentence;
	private int m_iResetAdCount = 0;

	#endregion

	#region Constructor

	public SceneOptions() {
		Reset ();
	}
		
	public SceneOptions(SceneOptions options) {
		InheritOptions (options);
	}

	#endregion

	#region Methods

	public void Reset() {
		m_oGenerationType = m_oResetGenType;
		m_iAdCount = m_iResetAdCount;
	}

	public void InheritOptions(SceneOptions options) {
		m_oGenerationType = options.GenType;
		m_iAdCount = options.AdCount;
	}

	#endregion

	#region Properties

	public GenerationType GenType {
		get {
			return m_oGenerationType;
		}
		set {
			m_oGenerationType = value;
		}
	}

	public int AdCount {
		get {
			return m_iAdCount;
		}
		set {
			m_iAdCount = value;
		}
	}

	#endregion
}

public enum GenerationType
{
	CompleteSentence,
	FullRandom
}
