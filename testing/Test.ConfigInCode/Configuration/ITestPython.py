from Test.ConfigInCode import ITestPython
import System

class TestPython(ITestPython):
	def get_ConstructorValue(self):
		return "HelloWorld"

class TestPythonWithConst(ITestPython):
	def __new__(cls, *args):
		cls.passedInValue = args[0]
		return System.Object.__new__(cls)
	def get_ConstructorValue(self):
		return self.passedInValue
