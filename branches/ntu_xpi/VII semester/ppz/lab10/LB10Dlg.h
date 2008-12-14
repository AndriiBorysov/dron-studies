// LB10Dlg.h : header file
//

#pragma once
#include "afxwin.h"


// CLB10Dlg dialog
class CLB10Dlg : public CDialog
{
// Construction
public:
	CLB10Dlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_LB10_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnEnChangeRichedit21();
	CRichEditCtrl richEdit;
	CEdit editBalance;
	CDateTimeCtrl fileTimeBegin;
	CDateTimeCtrl fileTimeEnd;
	afx_msg void OnBnClickedButton1();
	afx_msg void OnDtnCloseupDatetimepickerbegin(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnDtnCloseupDatetimepickerend(NMHDR *pNMHDR, LRESULT *pResult);
};
