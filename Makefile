NAME := konnect-tpp
REPO ?= 120268890679.dkr.ecr.us-west-2.amazonaws.com
REPO_URL ?= $(REPO)/koin-prod-ecr/
BUILD_ARGS:= $(BUILD_ARGS)
VERSION ?= _local
CONTEXT ?= local
BUILDS = $(NAME)
PROJECT ?= all
TARGET ?= dev
TAGS ?=
THIS_FILE := $(lastword $(MAKEFILE_LIST))

docker: $(BUILDS)

PROJECT_FOLDER=$(PROJECT)
ifeq ($(PROJECT),all)
PROJECT_FOLDER=
endif
GIT_SHA = $(shell git rev-parse HEAD)
GIT_STATUS = $(shell git status $(PROJECT_FOLDER) --porcelain)
ifneq ("$(GIT_STATUS)","")
GIT_SHASUFFIX=-dirty
endif
ifeq ("$(VERSION)","_local")
VERSION=$(GIT_SHA)$(GIT_SHASUFFIX)
endif

_validate_project:
ifneq ($(findstring $(PROJECT),"$(BUILDS) all"), $(PROJECT))
	@echo "Project $(PROJECT) not found"
	@exit 1
endif

$(BUILDS): _validate_project dockercheck
	DOCKER_BUILDKIT=1 docker build $(BUILD_ARGS) --ssh default --build-arg BUILDKIT_INLINE_CACHE=1 --build-arg BUILD_SHA=$(VERSION) -t $(REPO_URL)$@:$(VERSION) -f Dockerfile .
	$(foreach tag,$(TAGS), docker tag $(REPO_URL)$@:$(VERSION) $(REPO_URL)$@:${tag} ; )

loadenv:
-include .makerc
export

awscheck: loadenv
	@which aws >/dev/null; \
	if [ $$? -ne 0 ] ; then \
		echo 'aws binary not found. Please install using: https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-install.html' ; \
		exit 1 ; \
	fi

dockercheck:
	@which docker >/dev/null; \
	if [ $$? -ne 0 ] ; then \
		echo 'docker binary not found. Please install from: https://www.docker.com/community-edition' ; \
		exit 1 ; \
	fi

login: dockercheck awscheck loadenv
	@(aws ecr get-login-password --region us-west-2 | docker login --username AWS --password-stdin $(REPO)) 2> /dev/null || `aws ecr get-login --no-include-email --region us-west-2` ;
	@if [ $$? -ne 0 ] ; then \
		echo 'AWS ECR login failed. Please install the aws cli tools and run "aws configure" to set your credentials' ; \
		exit 1 ; \
	fi

createrepos: awscheck
	$(foreach build,$(BUILDS), aws ecr create-repository --region=us-west-2 --repository-name koin-prod-ecr/$(build) --image-scanning-configuration scanOnPush=true ; )

push: _validate_project loadenv login
ifeq ($(PROJECT),all)
	$(foreach build,$(BUILDS), docker push $(REPO_URL)$(build):$(VERSION) && $(foreach tag,$(TAGS), docker push $(REPO_URL)$(build):${tag} && ) ) /bin/true
else
	docker push $(REPO_URL)$(PROJECT):$(VERSION)
	$(foreach tag,$(TAGS), docker push $(REPO_URL)$(PROJECT):${tag} ; )
endif

_set_project_label:
PROJECT_LABEL=`echo "$(PROJECT)" | sed -e 's|/|-|g'`

.PHONY: docker _validate_project $(BUILDS) $(PARENT_CONTEXT_BUILDS) loadenv dockercheck login push

