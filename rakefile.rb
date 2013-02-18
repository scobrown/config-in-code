require 'albacore'
require 'yaml'
require './tools/ilmerge'

outdir = File.expand_path("./output/")

desc "Build the Configuration assembly"
task :default => [:buildRelease]

msbuild :buildRelease do |msb|
	msb.properties = {:configuration => :Release, :TrackFileAccess => false}
	msb.targets :clean, :Build
	msb.solution = "./source/ConfigInCode.sln"
end
nunit :unit_tests => :buildRelease do |nunit|
	Dir.mkdir outdir unless Dir.exists?(outdir)
	nunitRunner = Dir.glob("source/packages/NUnit.Runners.*/tools/nunit-console.exe")[0]
	nunitTests = Dir.glob("testing/Test.*/**/Test.*.dll")
	nunit.command = nunitRunner
	nunit.assemblies nunitTests
	nunit.options '/nologo' " /xml=#{File.join(outdir, 'nunit-test-results.xml')}"
	puts("Output to #{outdir}")
end
