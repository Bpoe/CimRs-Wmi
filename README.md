# CimRs-Wmi
This is the start of an implementation of a gateway/bridge service that will receive CIM-RS calls and proxy them to WMI. The WMI result will be translated back into a CIM-RS payload response.

## What is CIM-RS?
CIM-RS is Common Information Model over RESTful Services. Its part of the DMTF's WBEM (Web Based Enterprise Management) standard.

[Working Group](https://www.dmtf.org/standards/cimrs)

[Protocol](https://www.dmtf.org/sites/default/files/standards/documents/DSP0210_2.0.0.pdf)

[Payload](https://www.dmtf.org/sites/default/files/standards/documents/DSP0211_2.0.0.pdf)
